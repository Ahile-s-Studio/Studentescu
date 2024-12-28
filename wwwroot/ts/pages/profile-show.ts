// followButtons.ts

import {
    followUser,
    isFollowingUser,
    isPendingRequestSent,
    unfollowUser,
} from "../utils/followService.js";

document.addEventListener("DOMContentLoaded", () => {
    const followButtons =
        document.querySelectorAll<HTMLButtonElement>(".follow-button");

    followButtons.forEach(async (button) => {
        const userId = button.getAttribute("data-user-id");
        const updateText = async (button: HTMLButtonElement) => {
            const isFollowing = await isFollowingUser(userId);
            const isPendingRequest = await isPendingRequestSent(userId);
            if (isPendingRequest) {
                button.textContent = "Cancel Request";
            } else {
                button.textContent = isFollowing ? "Unfollow" : "Follow";
            }
        };
        updateText(button);

        button.addEventListener("click", async (event) => {
            updateText(button);
            if (userId) {
                if (button.textContent === "Follow") {
                    await followUser(userId);
                } else if (button.textContent === "Unfollow") {
                    await unfollowUser(userId);
                } else if (button.textContent === "Cancel Request") {
                    await unfollowUser(userId);
                }
            }
            updateText(button);
        });
    });
});
