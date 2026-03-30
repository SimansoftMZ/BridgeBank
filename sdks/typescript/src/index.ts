// Re-export main client
export { createBridgeBankClient, BridgeBankClientUriTemplate, BridgeBankClientNavigationMetadata } from './bridgeBankClient.js';
export type { BridgeBankClient } from './bridgeBankClient.js';

// Re-export models
export * from './models/index.js';

// Re-export API builders
export * from './api/index.js';
export * from './health/index.js';
