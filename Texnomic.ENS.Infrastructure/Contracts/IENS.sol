// SPDX-License-Identifier: MIT
pragma solidity ^0.8.27;

/**
 * @title ENS - Ethereum Name Service Interface
 * @dev The ENS registry interface for managing domain ownership and resolution.
 */
interface IENS {
    /**
     * @dev Emitted when the owner of a node assigns a new owner to a subnode.
     * @param node The parent node.
     * @param label The hash of the label specifying the subnode.
     * @param owner The new owner of the subnode.
     */
    event NewOwner(bytes32 indexed node, bytes32 indexed label, address owner);

    /**
     * @dev Emitted when the owner of a node transfers ownership to a new account.
     * @param node The node that was transferred.
     * @param owner The new owner.
     */
    event Transfer(bytes32 indexed node, address owner);

    /**
     * @dev Emitted when the resolver for a node changes.
     * @param node The node whose resolver changed.
     * @param resolver The new resolver address.
     */
    event NewResolver(bytes32 indexed node, address resolver);

    /**
     * @dev Emitted when the TTL of a node changes.
     * @param node The node whose TTL changed.
     * @param ttl The new TTL.
     */
    event NewTTL(bytes32 indexed node, uint64 ttl);

    /**
     * @dev Emitted when an operator is added or removed.
     * @param owner The owner whose operator status changed.
     * @param operator The operator address.
     * @param approved Whether the operator is approved.
     */
    event ApprovalForAll(
        address indexed owner,
        address indexed operator,
        bool approved
    );

    /**
     * @notice Sets the record for a node.
     * @param node The node to update.
     * @param owner The address of the new owner.
     * @param resolver The address of the resolver.
     * @param ttl The TTL in seconds.
     */
    function setRecord(
        bytes32 node,
        address owner,
        address resolver,
        uint64 ttl
    ) external;

    /**
     * @notice Sets the record for a subnode.
     * @param node The parent node.
     * @param label The hash of the label specifying the subnode.
     * @param owner The address of the new owner.
     * @param resolver The address of the resolver.
     * @param ttl The TTL in seconds.
     */
    function setSubnodeRecord(
        bytes32 node,
        bytes32 label,
        address owner,
        address resolver,
        uint64 ttl
    ) external;

    /**
     * @notice Transfers ownership of a node to a new address.
     * @param node The node to transfer ownership of.
     * @param owner The address of the new owner.
     */
    function setOwner(bytes32 node, address owner) external;

    /**
     * @notice Transfers ownership of a subnode.
     * @param node The parent node.
     * @param label The hash of the label specifying the subnode.
     * @param owner The address of the new owner.
     */
    function setSubnodeOwner(
        bytes32 node,
        bytes32 label,
        address owner
    ) external;

    /**
     * @notice Sets the resolver address for the specified node.
     * @param node The node to update.
     * @param resolver The address of the resolver.
     */
    function setResolver(bytes32 node, address resolver) external;

    /**
     * @notice Sets the TTL for the specified node.
     * @param node The node to update.
     * @param ttl The TTL in seconds.
     */
    function setTTL(bytes32 node, uint64 ttl) external;

    /**
     * @notice Enable or disable approval for a third party ("operator") to manage
     *         all of `msg.sender`'s ENS records.
     * @param operator Address to add to the set of authorized operators.
     * @param approved True if the operator is approved, false to revoke approval.
     */
    function setApprovalForAll(address operator, bool approved) external;

    /**
     * @notice Returns the address that owns the specified node.
     * @param node The specified node.
     * @return The address of the owner.
     */
    function owner(bytes32 node) external view returns (address);

    /**
     * @notice Returns the address of the resolver for the specified node.
     * @param node The specified node.
     * @return The address of the resolver.
     */
    function resolver(bytes32 node) external view returns (address);

    /**
     * @notice Returns the TTL of a node.
     * @param node The specified node.
     * @return The TTL of the node.
     */
    function ttl(bytes32 node) external view returns (uint64);

    /**
     * @notice Returns whether a record has been imported to the registry.
     * @param node The specified node.
     * @return True if the record exists, false otherwise.
     */
    function recordExists(bytes32 node) external view returns (bool);

    /**
     * @notice Query if an address is an authorized operator for another address.
     * @param owner The address that owns the records.
     * @param operator The address that acts on behalf of the owner.
     * @return True if `operator` is an approved operator for `owner`, false otherwise.
     */
    function isApprovedForAll(
        address owner,
        address operator
    ) external view returns (bool);
}
