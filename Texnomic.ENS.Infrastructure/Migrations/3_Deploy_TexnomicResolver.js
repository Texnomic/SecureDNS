const TexnomicResolver = artifacts.require("TexnomicResolver");

module.exports = function(deployer) 
{
  deployer.deploy(TexnomicResolver, "0x314159265dd8dbb310642f98f50c066173c1259b");
};