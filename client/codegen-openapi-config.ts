import type { ConfigFile } from '@rtk-query/codegen-openapi'

const config: ConfigFile = {
  schemaFile: './expertbridge-openapi.json',
  apiFile: './src/app/emptyApi.ts',
  apiImport: 'emptyApiSlice',
  outputFile: './src/features/api/appApiSlice.ts',
  exportName: 'appApiSlice',
  hooks: true,
};

export default config;

// module.exports = {
//   schemaFile: './expertbridge-openapi.json',
//   apiFile: './src/app/emptyApi.ts',
//   apiImport: 'emptyApiSlice',
//   outputFile: './src/features/api/appApiSlice.ts',
//   exportName: 'appApiSlice',
//   hooks: true,
// };

