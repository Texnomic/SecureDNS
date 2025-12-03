import { ethers } from "hardhat";
import * as fs from "fs";
import * as path from "path";

/**
 * @notice Script to verify all deployed contracts on Etherscan
 * @dev Reads deployment data and verifies each contract
 */
async function main() {
  console.log("Starting contract verification...\n");

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

  const contracts = deploymentData.contracts;

  // Verify ENS Registry (no constructor args)
  console.log("1. Verifying ENS Registry...");
  try {
    await run("verify:verify", {
      address: contracts.ENSRegistry,
      constructorArguments: [],
    });
    console.log("? ENS Registry verified\n");
  } catch (error: any) {
    console.log(`? Failed: ${error.message}\n`);
  }

  // Verify FIFS Registrar
  console.log("2. Verifying FIFS Registrar...");
  try {
    const ethNode = ethers.namehash("eth");
    await run("verify:verify", {
      address: contracts.FIFSRegistrar,
      constructorArguments: [contracts.ENSRegistry, ethNode],
    });
    console.log("? FIFS Registrar verified\n");
  } catch (error: any) {
    console.log(`? Failed: ${error.message}\n`);
  }

  // Verify Public Resolver
  console.log("3. Verifying Public Resolver...");
  try {
    await run("verify:verify", {
      address: contracts.PublicResolver,
      constructorArguments: [contracts.ENSRegistry],
    });
    console.log("? Public Resolver verified\n");
  } catch (error: any) {
    console.log(`? Failed: ${error.message}\n`);
  }

  // Verify Reverse Registrar
  console.log("4. Verifying Reverse Registrar...");
  try {
    await run("verify:verify", {
      address: contracts.ReverseRegistrar,
      constructorArguments: [contracts.ENSRegistry],
    });
    console.log("? Reverse Registrar verified\n");
  } catch (error: any) {
    console.log(`? Failed: ${error.message}\n`);
  }

  console.log("Verification process completed!");
}

// Required for running Hardhat tasks
async function run(task: string, args: any) {
  const hre = require("hardhat");
  return hre.run(task, args);
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
