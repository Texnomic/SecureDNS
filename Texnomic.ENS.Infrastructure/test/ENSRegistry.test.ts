import { expect } from "chai";
import { ethers } from "hardhat";
import { Contract, Signer } from "ethers";
import { loadFixture } from "@nomicfoundation/hardhat-network-helpers";

describe("ENS Registry", function () {
  // Fixture for deploying the ENS Registry
  async function deployENSRegistryFixture() {
    const [owner, addr1, addr2] = await ethers.getSigners();

    const ENSRegistry = await ethers.getContractFactory("ENSRegistry");
    const registry = await ENSRegistry.deploy();
    await registry.waitForDeployment();

    return { registry, owner, addr1, addr2 };
  }

  describe("Deployment", function () {
    it("Should set the right owner for the root node", async function () {
      const { registry, owner } = await loadFixture(deployENSRegistryFixture);
      expect(await registry.owner(ethers.ZeroHash)).to.equal(owner.address);
    });

    it("Should have zero address for non-existent nodes", async function () {
      const { registry } = await loadFixture(deployENSRegistryFixture);
      const randomNode = ethers.id("random");
      expect(await registry.owner(randomNode)).to.equal(ethers.ZeroAddress);
    });
  });

  describe("Ownership", function () {
    it("Should allow owner to transfer ownership", async function () {
      const { registry, owner, addr1 } = await loadFixture(deployENSRegistryFixture);
      
      await expect(registry.setOwner(ethers.ZeroHash, addr1.address))
        .to.emit(registry, "Transfer")
        .withArgs(ethers.ZeroHash, addr1.address);

      expect(await registry.owner(ethers.ZeroHash)).to.equal(addr1.address);
    });

    it("Should not allow non-owner to transfer ownership", async function () {
      const { registry, addr1, addr2 } = await loadFixture(deployENSRegistryFixture);
      
      await expect(
        registry.connect(addr1).setOwner(ethers.ZeroHash, addr2.address)
      ).to.be.revertedWithCustomError(registry, "Unauthorized");
    });

    it("Should allow creating subnodes", async function () {
      const { registry, owner, addr1 } = await loadFixture(deployENSRegistryFixture);
      
      const label = ethers.id("test");
      const expectedSubnode = ethers.namehash("test");

      await expect(registry.setSubnodeOwner(ethers.ZeroHash, label, addr1.address))
        .to.emit(registry, "NewOwner")
        .withArgs(ethers.ZeroHash, label, addr1.address);

      expect(await registry.owner(expectedSubnode)).to.equal(addr1.address);
    });
  });

  describe("Resolver Management", function () {
    it("Should allow owner to set resolver", async function () {
      const { registry, owner, addr1 } = await loadFixture(deployENSRegistryFixture);
      
      await expect(registry.setResolver(ethers.ZeroHash, addr1.address))
        .to.emit(registry, "NewResolver")
        .withArgs(ethers.ZeroHash, addr1.address);

      expect(await registry.resolver(ethers.ZeroHash)).to.equal(addr1.address);
    });

    it("Should not allow non-owner to set resolver", async function () {
      const { registry, addr1, addr2 } = await loadFixture(deployENSRegistryFixture);
      
      await expect(
        registry.connect(addr1).setResolver(ethers.ZeroHash, addr2.address)
      ).to.be.revertedWithCustomError(registry, "Unauthorized");
    });
  });

  describe("TTL Management", function () {
    it("Should allow owner to set TTL", async function () {
      const { registry } = await loadFixture(deployENSRegistryFixture);
      
      const ttl = 3600; // 1 hour

      await expect(registry.setTTL(ethers.ZeroHash, ttl))
        .to.emit(registry, "NewTTL")
        .withArgs(ethers.ZeroHash, ttl);

      expect(await registry.ttl(ethers.ZeroHash)).to.equal(ttl);
    });
  });

  describe("Operator Approval", function () {
    it("Should allow setting operator approval", async function () {
      const { registry, owner, addr1 } = await loadFixture(deployENSRegistryFixture);
      
      await expect(registry.setApprovalForAll(addr1.address, true))
        .to.emit(registry, "ApprovalForAll")
        .withArgs(owner.address, addr1.address, true);

      expect(await registry.isApprovedForAll(owner.address, addr1.address)).to.be.true;
    });

    it("Should allow approved operator to manage records", async function () {
      const { registry, owner, addr1, addr2 } = await loadFixture(deployENSRegistryFixture);
      
      // Approve addr1 as operator
      await registry.setApprovalForAll(addr1.address, true);

      // addr1 should be able to set resolver
      await expect(
        registry.connect(addr1).setResolver(ethers.ZeroHash, addr2.address)
      ).to.not.be.reverted;

      expect(await registry.resolver(ethers.ZeroHash)).to.equal(addr2.address);
    });
  });

  describe("Record Management", function () {
    it("Should set complete record", async function () {
      const { registry, addr1, addr2 } = await loadFixture(deployENSRegistryFixture);
      
      const ttl = 3600;

      await expect(registry.setRecord(ethers.ZeroHash, addr1.address, addr2.address, ttl))
        .to.emit(registry, "Transfer")
        .withArgs(ethers.ZeroHash, addr1.address)
        .and.to.emit(registry, "NewResolver")
        .withArgs(ethers.ZeroHash, addr2.address)
        .and.to.emit(registry, "NewTTL")
        .withArgs(ethers.ZeroHash, ttl);

      expect(await registry.owner(ethers.ZeroHash)).to.equal(addr1.address);
      expect(await registry.resolver(ethers.ZeroHash)).to.equal(addr2.address);
      expect(await registry.ttl(ethers.ZeroHash)).to.equal(ttl);
    });

    it("Should set subnode record", async function () {
      const { registry, addr1, addr2 } = await loadFixture(deployENSRegistryFixture);
      
      const label = ethers.id("test");
      const expectedSubnode = ethers.namehash("test");
      const ttl = 3600;

      await registry.setSubnodeRecord(
        ethers.ZeroHash,
        label,
        addr1.address,
        addr2.address,
        ttl
      );

      expect(await registry.owner(expectedSubnode)).to.equal(addr1.address);
      expect(await registry.resolver(expectedSubnode)).to.equal(addr2.address);
      expect(await registry.ttl(expectedSubnode)).to.equal(ttl);
    });
  });

  describe("Record Existence", function () {
    it("Should return true for existing records", async function () {
      const { registry } = await loadFixture(deployENSRegistryFixture);
      expect(await registry.recordExists(ethers.ZeroHash)).to.be.true;
    });

    it("Should return false for non-existent records", async function () {
      const { registry } = await loadFixture(deployENSRegistryFixture);
      const randomNode = ethers.id("nonexistent");
      expect(await registry.recordExists(randomNode)).to.be.false;
    });
  });
});
