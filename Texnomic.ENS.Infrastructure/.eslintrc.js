module.exports = {
  extends: [
    "eslint:recommended",
    "plugin:@typescript-eslint/recommended",
    "plugin:prettier/recommended",
  ],
  parser: "@typescript-eslint/parser",
  parserOptions: {
    ecmaVersion: 2022,
    sourceType: "module",
    project: "./tsconfig.json",
  },
  plugins: ["@typescript-eslint", "prettier"],
  rules: {
    "@typescript-eslint/no-explicit-any": "warn",
    "@typescript-eslint/explicit-function-return-type": "off",
    "@typescript-eslint/no-unused-vars": [
      "error",
      { argsIgnorePattern: "^_" },
    ],
    "prettier/prettier": "error",
  },
  env: {
    node: true,
    mocha: true,
    es2022: true,
  },
  ignorePatterns: [
    "node_modules/",
    "artifacts/",
    "cache/",
    "coverage/",
    "typechain-types/",
    "dist/",
    "build/",
  ],
};