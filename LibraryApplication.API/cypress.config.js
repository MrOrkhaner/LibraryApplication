module.exports = {
  e2e: {
    experimentalFetchPolyfill: true, // ✅ Add this line
    setupNodeEvents(on, config) {
      // implement node event listeners here
    },
  },
};
