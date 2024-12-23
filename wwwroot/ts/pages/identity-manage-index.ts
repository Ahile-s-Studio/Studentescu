document
    .querySelector<HTMLInputElement>("#isProfilePrivateSwitch")
    ?.addEventListener("change", function () {
        const label = document.querySelector<HTMLLabelElement>(
            'label[for="isProfilePrivateSwitch"]'
        );
        if (label == null || label.textContent === "") return;
        label.textContent = this.checked ? "Private" : "Public";
    });
