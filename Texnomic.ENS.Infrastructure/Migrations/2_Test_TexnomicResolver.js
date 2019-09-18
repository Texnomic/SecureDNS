const ENSRegistry = artifacts.require("ENSRegistry");
const TexnomicResolver = artifacts.require("TexnomicResolver");

module.exports = function(deployer) 
{
    deployer.deploy(ENSRegistry)
    .then(function(ENSRegistryInstance) 
    {
      return deployer.deploy(TexnomicResolver, ENSRegistryInstance.address);
    });
};