import { ethers } from "hardhat";
import * as fs from "fs";
import * as path from "path";

/**
 * @notice Script to interact with deployed ENS contracts
 * @dev Provides examples of common ENS operations
 */
async function main() {
  console.log("ENS Contract Interaction Examples\n");

  // Load deployment addresses
  const deploymentsDir = path.join(__dirname, "..", "deployments");
  const files = fs.readdirSync(deploymentsDir);
  const latestDeployment = files
    .filter((f) => f.startsWith("deployment-"))
    .sort()
    .reverse()[0];

  if (!latestDeployment) {
    console.error("No deployment found. Please deploy contracts first.");
    return;
  }

  const deploymentData = JSON.parse(
    fs.readFileSync(path.join(deploymentsDir, latestDeployment), "utf8")
  );

  console.log(`Using deployment: ${latestDeployment}`);
  console.log(`Network: ${deploymentData.network}`);
  console.log(`Chain ID: ${deploymentData.chainId}\n`);

  const [signer] = await ethers.getSigners();
  console.log(`Interacting as: ${signer.address}\n`);

  // Get contract instances
  const registry = await ethers.getContractAt(
    "ENSRegistry",
    deploymentData.contracts.ENSRegistry
  );

  const resolver = await ethers.getContractAt(
    "PublicResolver",
    deploymentData.contracts.PublicResolver
  );

  // Example 1: Register a domain
  console.log("Example 1: Registering a domain");
  console.log("-".repeat(50));

  const domainName = "example";
  const domainLabel = ethers.id(domainName);
  const domainNode = ethers.namehash(`${domainName}.eth`);

  try {
    const tx1 = await registry.setSubnodeOwner(
      ethers.namehash("eth"),
      domainLabel,
      signer.address
    );
    await tx1.wait();
    console.log(`? Registered ${domainName}.eth`);
    console.log(`  Transaction: ${tx1.hash}`);
  } catch (error: any) {
    console.log(`? Failed to register: ${error.message}`);
  }

  // Example 2: Set resolver
  console.log("\nExample 2: Setting resolver");
  console.log("-".repeat(50));

  try {
    const tx2 = await registry.setResolver(
      domainNode,
      deploymentData.contracts.PublicResolver
    );
    await tx2.wait();
    console.log(`? Set resolver for ${domainName}.eth`);
    console.log(`  Resolver: ${deploymentData.contracts.PublicResolver}`);
  } catch (error: any) {
    console.log(`? Failed to set resolver: ${error.message}`);
  }

  // Example 3: Set address record
  console.log("\nExample 3: Setting address record");
  console.log("-".repeat(50));

  try {
    const targetAddress = signer.address;
    const tx3 = await resolver["setAddr(bytes32,address)"](
      domainNode,
      targetAddress
    );
    await tx3.wait();
    console.log(`? Set address for ${domainName}.eth ? ${targetAddress}`);
  } catch (error: any) {
    console.log(`? Failed to set address: ${error.message}`);
  }

  // Example 4: Resolve address
  console.log("\nExample 4: Resolving address");
  console.log("-".repeat(50));

  try {
    const resolvedAddress = await resolver["addr(bytes32)"](domainNode);
    console.log(`? Resolved ${domainName}.eth ? ${resolvedAddress}`);
  } catch (error: any) {
    console.log(`? Failed to resolve: ${error.message}`);
  }

  // Example 5: Set text record
  console.log("\nExample 5: Setting text record");
  console.log("-".repeat(50));

  try {
    const tx5 = await resolver.setText(
      domainNode,
      "description",
      "Texnomic ENS Infrastructure"
    );
    await tx5.wait();
    console.log(`? Set text record: description`);

    const textValue = await resolver.text(domainNode, "description");
    console.log(`  Value: ${textValue}`);
  } catch (error: any) {
    console.log(`? Failed to set text: ${error.message}`);
  }

  // Example 6: Check domain ownership
  console.log("\nExample 6: Checking ownership");
  console.log("-".repeat(50));

  try {
    const owner = await registry.owner(domainNode);
    console.log(`? Owner of ${domainName}.eth: ${owner}`);
    console.log(`  Is signer: ${owner === signer.address}`);
  } catch (error: any) {
    console.log(`? Failed to check ownership: ${error.message}`);
  }

  console.log("\n" + "=".repeat(50));
  console.log("Interaction examples completed!");
  console.log("=".repeat(50));
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
