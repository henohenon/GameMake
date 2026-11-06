(function () {
  "use strict";

  var cfg = JSON.parse(document.getElementById("unity-build-config").textContent);
  var container = document.getElementById("unity-container");
  var canvas = document.getElementById("unity-canvas");
  var loading = document.getElementById("loading");
  var loadingFill = document.getElementById("loading-bar-fill");
  var loadingText = document.getElementById("loading-text");
  var banner = document.getElementById("banner");

  // Keep the game's aspect ratio (the UI was laid out for a fixed resolution)
  // and letterbox it inside whatever size the Discord frame gives us.
  var aspect = cfg.width / cfg.height;
  function fit() {
    var w = container.clientWidth;
    var h = container.clientHeight;
    if (w / h > aspect) w = Math.floor(h * aspect);
    else h = Math.floor(w / aspect);
    canvas.style.width = w + "px";
    canvas.style.height = h + "px";
  }
  window.addEventListener("resize", fit);
  fit();

  function showBanner(msg, type) {
    var div = document.createElement("div");
    div.textContent = msg;
    if (type === "error") div.className = "error";
    banner.appendChild(div);
    if (type !== "error") {
      setTimeout(function () {
        banner.removeChild(div);
      }, 5000);
    }
  }

  var config = {
    dataUrl: cfg.dataUrl,
    frameworkUrl: cfg.frameworkUrl,
    codeUrl: cfg.codeUrl,
    streamingAssetsUrl: cfg.streamingAssetsUrl,
    companyName: cfg.companyName,
    productName: cfg.productName,
    productVersion: cfg.productVersion,
    showBanner: showBanner,
  };

  var script = document.createElement("script");
  script.src = cfg.loaderUrl;
  script.onload = function () {
    createUnityInstance(canvas, config, function (progress) {
      loadingFill.style.width = 100 * progress + "%";
      if (progress >= 0.9) loadingText.textContent = "起動中…";
    })
      .then(function (unityInstance) {
        window.unityInstance = unityInstance;
        loading.classList.add("done");
        canvas.focus();
      })
      .catch(function (message) {
        loadingText.textContent = "読み込みに失敗しました";
        showBanner(String(message), "error");
      });
  };
  script.onerror = function () {
    loadingText.textContent = "読み込みに失敗しました";
  };
  document.body.appendChild(script);
})();
