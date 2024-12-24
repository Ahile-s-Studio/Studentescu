"use strict";
var _a;
(_a = document.querySelector("#isProfilePrivateSwitch")) === null ||
_a === void 0
    ? void 0
    : _a.addEventListener("change", function () {
          var label = document.querySelector(
              'label[for="isProfilePrivateSwitch"]'
          );
          if (label == null || label.textContent === "") return;
          label.textContent = this.checked ? "Private" : "Public";
      });
