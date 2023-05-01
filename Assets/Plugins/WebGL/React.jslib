mergeInto(LibraryManager.library, {
  SetMove: function (position, move) {
    window.dispatchReactUnityEvent("SetMove", position, move);
  },
  ConnectWallet: function (position, move) {
    window.dispatchReactUnityEvent("ConnectWallet");
  },
  StartGame: function (position, move) {
    window.dispatchReactUnityEvent("StartGame");
  },
});