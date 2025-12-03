import { ethers } from "hardhat";
import { Contract, Signer } from "ethers";

/**
 * @notice Deployment script for ENS Registry and related contracts
 * @dev This script deploys the complete ENS infrastructure
 */
async function main() {
  console.log("Starting ENS Infrastructure deployment...");
  
  const [deployer] = await ethers.getSigners();
  console.log(`Deploying contracts with account: ${deployer.address}`);
  console.log(`Account balance: ${ethers.formatEther(await ethers.provider.getBalance(deployer.address))} ETH`);

  // Deploy ENS Registry
  console.log("\n1. Deploying ENS Registry...");
  const ENSRegistry = await ethers.getContractFactory("ENSRegistry");
  const ensRegistry = await ENSRegistry.deploy();
  await ensRegistry.waitForDeployment();
  const ensRegistryAddress = await ensRegistry.getAddress();
  console.log(`ENS Registry deployed to: ${ensRegistryAddress}`);

  // Deploy FIFS Registrar
  console.log("\n2. Deploying FIFS Registrar...");
  const FIFSRegistrar = await ethers.getContractFactory("FIFSRegistrar");
  const rootNode = ethers.namehash("eth"); // Using .eth TLD
  const fifsRegistrar = await FIFSRegistrar.deploy(ensRegistryAddress, rootNode);
  await fifsRegistrar.waitForDeployment();
  const fifsRegistrarAddress = await fifsRegistrar.getAddress();
  console.log(`FIFS Registrar deployed to: ${fifsRegistrarAddress}`);

  // Deploy Public Resolver
  console.log("\n3. Deploying Public Resolver...");
  const PublicResolver = await ethers.getContractFactory("PublicResolver");
  const publicResolver = await PublicResolver.deploy(ensRegistryAddress);
  await publicResolver.waitForDeployment();
  const publicResolverAddress = await publicResolver.getAddress();
  console.log(`Public Resolver deployed to: ${publicResolverAddress}`);

  // Deploy Reverse Registrar
  console.log("\n4. Deploying Reverse Registrar...");
  const ReverseRegistrar = await ethers.getContractFactory("ReverseRegistrar");
  const reverseRegistrar = await ReverseRegistrar.deploy(ensRegistryAddress);
  await reverseRegistrar.waitForDeployment();
  const reverseRegistrarAddress = await reverseRegistrar.getAddress();
  console.log(`Reverse Registrar deployed to: ${reverseRegistrarAddress}`);

  // Setup ownership
  console.log("\n5. Setting up ownership...");
  const tx1 = await ensRegistry.setSubnodeOwner(
    ethers.ZeroHash,
    ethers.id("eth"),
    fifsRegistrarAddress
  );
  await tx1.wait();
  console.log("Set FIFS Registrar as owner of .eth");

  const tx2 = await ensRegistry.setSubnodeOwner(
    ethers.ZeroHash,
    ethers.id("reverse"),
    deployer.address
  );
  await tx2.wait();
  console.log("Set deployer as owner of .reverse");

  const tx3 = await ensRegistry.setSubnodeOwner(
    ethers.namehash("reverse"),
    ethers.id("addr"),
    reverseRegistrarAddress
  );
  await tx3.wait();
  console.log("Set Reverse Registrar as owner of addr.reverse");

  // Summary
  console.log("\n========================================");
  console.log("Deployment Summary");
  console.log("========================================");
  console.log(`Network: ${(await ethers.provider.getNetwork()).name}`);
  console.log(`Chain ID: ${(await ethers.provider.getNetwork()).chainId}`);
  console.log(`Deployer: ${deployer.address}`);
  console.log(`\nContracts:`);
  console.log(`  ENS Registry: ${ensRegistryAddress}`);
  console.log(`  FIFS Registrar: ${fifsRegistrarAddress}`);
  console.log(`  Public Resolver: ${publicResolverAddress}`);
  console.log(`  Reverse Registrar: ${reverseRegistrarAddress}`);
  console.log("========================================\n");

  // Save deployment addresses
  const deployment = {
    network: (await ethers.provider.getNetwork()).name,
    chainId: Number((await ethers.provider.getNetwork()).chainId),
    deployer: deployer.address,
    timestamp: new Date().toISOString(),
    contracts: {
      ENSRegistry: ensRegistryAddress,
      FIFSRegistrar: fifsRegistrarAddress,
      PublicResolver: publicResolverAddress,
      ReverseRegistrar: reverseRegistrarAddress,
    },
  };

  const fs = require("fs");
  const path = require("path");
  const deploymentsDir = path.join(__dirname, "..", "deployments");
  
  if (!fs.existsSync(deploymentsDir)) {
    fs.mkdirSync(deploymentsDir, { recursive: true });
  }

  const filename = `deployment-${(await ethers.provider.getNetwork()).name}-${Date.now()}.json`;
  fs.writeFileSync(
    path.join(deploymentsDir, filename),
    JSON.stringify(deployment, null, 2)
  );
  console.log(`Deployment details saved to: deployments/${filename}`);
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
