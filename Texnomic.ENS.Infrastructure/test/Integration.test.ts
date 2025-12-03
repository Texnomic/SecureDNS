import { expect } from "chai";
import { ethers } from "hardhat";
import { loadFixture } from "@nomicfoundation/hardhat-network-helpers";

describe("Integration Tests", function () {
  async function deployFullENSFixture() {
    const [owner, user1, user2] = await ethers.getSigners();

    // Deploy ENS Registry
    const ENSRegistry = await ethers.getContractFactory("ENSRegistry");
    const registry = await ENSRegistry.deploy();
    await registry.waitForDeployment();

    // Deploy FIFS Registrar for .eth
    const FIFSRegistrar = await ethers.getContractFactory("FIFSRegistrar");
    const ethNode = ethers.namehash("eth");
    const registrar = await FIFSRegistrar.deploy(
      await registry.getAddress(),
      ethNode
    );
    await registrar.waitForDeployment();

    // Deploy Public Resolver
    const PublicResolver = await ethers.getContractFactory("PublicResolver");
    const resolver = await PublicResolver.deploy(await registry.getAddress());
    await resolver.waitForDeployment();

    // Setup .eth TLD
    await registry.setSubnodeOwner(
      ethers.ZeroHash,
      ethers.id("eth"),
      await registrar.getAddress()
    );

    return { registry, registrar, resolver, owner, user1, user2 };
  }

  describe("Complete Domain Registration Flow", function () {
    it("Should register, resolve, and transfer a domain", async function () {
      const { registry, registrar, resolver, owner, user1 } =
        await loadFixture(deployFullENSFixture);

      const domainName = "example";
      const domainLabel = ethers.id(domainName);
      const domainNode = ethers.namehash(`${domainName}.eth`);

      // Step 1: Register domain
      await expect(registrar.register(domainLabel, owner.address))
        .to.emit(registry, "NewOwner")
        .withArgs(ethers.namehash("eth"), domainLabel, owner.address);

      expect(await registry.owner(domainNode)).to.equal(owner.address);

      // Step 2: Set resolver
      await registry.setResolver(domainNode, await resolver.getAddress());

      expect(await registry.resolver(domainNode)).to.equal(
        await resolver.getAddress()
      );

      // Step 3: Set address in resolver
      await resolver["setAddr(bytes32,address)"](domainNode, user1.address);

      expect(await resolver["addr(bytes32)"](domainNode)).to.equal(
        user1.address
      );

      // Step 4: Transfer domain
      await registry.setOwner(domainNode, user1.address);

      expect(await registry.owner(domainNode)).to.equal(user1.address);
    });

    it("Should handle subdomain registration", async function () {
      const { registry, registrar, resolver, owner, user1 } =
        await loadFixture(deployFullENSFixture);

      // Register parent domain
      const parentLabel = ethers.id("parent");
      const parentNode = ethers.namehash("parent.eth");
      await registrar.register(parentLabel, owner.address);

      // Register subdomain
      const subLabel = ethers.id("sub");
      const subNode = ethers.namehash("sub.parent.eth");

      await registry.setSubnodeOwner(parentNode, subLabel, user1.address);

      expect(await registry.owner(subNode)).to.equal(user1.address);

      // Set resolver for subdomain
      await registry.connect(user1).setResolver(
        subNode,
        await resolver.getAddress()
      );

      expect(await registry.resolver(subNode)).to.equal(
        await resolver.getAddress()
      );
    });
  });

  describe("Resolver Functionality", function () {
    it("Should set and get multiple record types", async function () {
      const { registry, registrar, resolver, owner } = await loadFixture(
        deployFullENSFixture
      );

      const domainLabel = ethers.id("multi");
      const domainNode = ethers.namehash("multi.eth");

      await registrar.register(domainLabel, owner.address);
      await registry.setResolver(domainNode, await resolver.getAddress());

      // Set address
      const testAddress = "0x1234567890123456789012345678901234567890";
      await resolver["setAddr(bytes32,address)"](domainNode, testAddress);

      // Set text records
      await resolver.setText(domainNode, "email", "test@example.com");
      await resolver.setText(domainNode, "url", "https://example.com");
      await resolver.setText(domainNode, "description", "Test Domain");

      // Verify all records
      expect(await resolver["addr(bytes32)"](domainNode)).to.equal(
        testAddress
      );
      expect(await resolver.text(domainNode, "email")).to.equal(
        "test@example.com"
      );
      expect(await resolver.text(domainNode, "url")).to.equal(
        "https://example.com"
      );
      expect(await resolver.text(domainNode, "description")).to.equal(
        "Test Domain"
      );
    });
  });

  describe("Security and Authorization", function () {
    it("Should prevent unauthorized domain modification", async function () {
      const { registry, registrar, resolver, owner, user1 } =
        await loadFixture(deployFullENSFixture);

      const domainLabel = ethers.id("secure");
      const domainNode = ethers.namehash("secure.eth");

      await registrar.register(domainLabel, owner.address);

      // Unauthorized user should not be able to set resolver
      await expect(
        registry.connect(user1).setResolver(
          domainNode,
          await resolver.getAddress()
        )
      ).to.be.revertedWithCustomError(registry, "Unauthorized");
    });

    it("Should allow operator to manage domains", async function () {
      const { registry, registrar, resolver, owner, user1 } =
        await loadFixture(deployFullENSFixture);

      const domainLabel = ethers.id("operator");
      const domainNode = ethers.namehash("operator.eth");

      await registrar.register(domainLabel, owner.address);

      // Set user1 as operator
      await registry.setApprovalForAll(user1.address, true);

      // Operator should be able to set resolver
      await expect(
        registry.connect(user1).setResolver(
          domainNode,
          await resolver.getAddress()
        )
      ).to.not.be.reverted;

      expect(await registry.resolver(domainNode)).to.equal(
        await resolver.getAddress()
      );
    });
  });
});
