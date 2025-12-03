import { ethers } from "hardhat";

/**
 * @notice Utility script to check current gas prices on the network
 * @dev Helps determine optimal gas price for deployment
 */
async function main() {
  console.log("Fetching current gas prices...\n");

  const provider = ethers.provider;
  const feeData = await provider.getFeeData();

  console.log("=".repeat(50));
  console.log("Gas Price Information");
  console.log("=".repeat(50));

  if (feeData.gasPrice) {
    const gasPriceGwei = ethers.formatUnits(feeData.gasPrice, "gwei");
    console.log(`Gas Price: ${gasPriceGwei} Gwei`);
  }

  if (feeData.maxFeePerGas && feeData.maxPriorityFeePerGas) {
    const maxFeeGwei = ethers.formatUnits(feeData.maxFeePerGas, "gwei");
    const maxPriorityFeeGwei = ethers.formatUnits(
      feeData.maxPriorityFeePerGas,
      "gwei"
    );

    console.log(`\nEIP-1559 Fees:`);
    console.log(`  Max Fee Per Gas: ${maxFeeGwei} Gwei`);
    console.log(`  Max Priority Fee: ${maxPriorityFeeGwei} Gwei`);
  }

  // Estimate deployment costs
  const estimatedGasUnits = 5000000; // Rough estimate for full deployment
  
  if (feeData.gasPrice) {
    const estimatedCost = feeData.gasPrice * BigInt(estimatedGasUnits);
    const estimatedCostEth = ethers.formatEther(estimatedCost);
    
    console.log(`\nEstimated Deployment Cost:`);
    console.log(`  Gas Units: ${estimatedGasUnits.toLocaleString()}`);
    console.log(`  Total Cost: ${estimatedCostEth} ETH`);
  }

  console.log("=".repeat(50));
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
