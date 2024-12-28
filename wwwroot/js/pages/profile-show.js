// followButtons.ts
var __awaiter =
    (this && this.__awaiter) ||
    function (thisArg, _arguments, P, generator) {
        function adopt(value) {
            return value instanceof P
                ? value
                : new P(function (resolve) {
                      resolve(value);
                  });
        }
        return new (P || (P = Promise))(function (resolve, reject) {
            function fulfilled(value) {
                try {
                    step(generator.next(value));
                } catch (e) {
                    reject(e);
                }
            }
            function rejected(value) {
                try {
                    step(generator["throw"](value));
                } catch (e) {
                    reject(e);
                }
            }
            function step(result) {
                result.done
                    ? resolve(result.value)
                    : adopt(result.value).then(fulfilled, rejected);
            }
            step(
                (generator = generator.apply(thisArg, _arguments || [])).next()
            );
        });
    };
import {
    followUser,
    isFollowingUser,
    isPendingRequestSent,
    unfollowUser,
} from "../utils/followService.js";
document.addEventListener("DOMContentLoaded", () => {
    const followButtons = document.querySelectorAll(".follow-button");
    followButtons.forEach((button) =>
        __awaiter(void 0, void 0, void 0, function* () {
            const userId = button.getAttribute("data-user-id");
            const updateText = (button) =>
                __awaiter(void 0, void 0, void 0, function* () {
                    const isFollowing = yield isFollowingUser(userId);
                    const isPendingRequest = yield isPendingRequestSent(userId);
                    if (isPendingRequest) {
                        button.textContent = "Cancel Request";
                    } else {
                        button.textContent = isFollowing
                            ? "Unfollow"
                            : "Follow";
                    }
                });
            updateText(button);
            button.addEventListener("click", (event) =>
                __awaiter(void 0, void 0, void 0, function* () {
                    updateText(button);
                    if (userId) {
                        if (button.textContent === "Follow") {
                            yield followUser(userId);
                        } else if (button.textContent === "Unfollow") {
                            yield unfollowUser(userId);
                        } else if (button.textContent === "Cancel Request") {
                            yield unfollowUser(userId);
                        }
                    }
                    updateText(button);
                })
            );
        })
    );
});
