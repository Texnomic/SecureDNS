const HDWalletProvider = require("@truffle/hdwallet-provider");
const Web3 = require("web3");

const web3 = new Web3();

const GetEnv = env => 
{
  const value = process.env[env];

  if (typeof value === 'undefined') 
  {
    throw new Error(`${env} has not been set.`);
  }

  return value;
};

const MNEMONIC = GetEnv('ETH_WALLET_MNEMONIC');
const LiveNetwork = GetEnv('ETH_LIVE_NETWORK');
const LiveNetworkID = GetEnv('ETH_LIVE_NETWORK_ID');


module.exports = {
  networks: 
  {
    "Ganache": 
    {
      host: "127.0.0.1",
      port: 7545,
      network_id: "5777",
      gas: 4000000
    },
    "Live": 
    {
      provider: () => new HDWalletProvider(MNEMONIC, LiveNetwork),
      network_id: LiveNetworkID,
      gasPrice: web3.utils.toWei('20', 'gwei')
    }
  }
};
