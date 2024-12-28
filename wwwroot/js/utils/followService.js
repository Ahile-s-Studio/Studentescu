// followService.ts
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
export function followUser(userId) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            const response = yield fetch(`/follow/${userId}`, {
                // const response = await fetch(`/following`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "same-origin",
            });
            if (!response.ok) {
                throw new Error(`Error: ${response.statusText}`);
            }
            console.log("User followed successfully.");
        } catch (error) {
            console.error("Error following user:", error);
        }
    });
}
export function unfollowUser(userId) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            const response = yield fetch(`/unfollow/${userId}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "same-origin",
            });
            if (!response.ok) {
                throw new Error(`Error: ${response.statusText}`);
            }
            console.log("User unfollowed successfully.");
        } catch (error) {
            console.error("Error unfollowing user:", error);
        }
    });
}
export function isFollowingUser(userId) {
    return __awaiter(this, void 0, void 0, function* () {
        if (userId === null) {
            return false;
        }
        try {
            const response = yield fetch("/following", {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                },
            });
            if (!response.ok) {
                throw new Error(`Error: ${response.statusText}`);
            }
            const followingUsers = yield response.json();
            return followingUsers.map((user) => user.id).indexOf(userId) != -1;
        } catch (error) {
            console.error("Error unfollowing user:", error);
        }
        return false;
    });
}
export function isPendingRequestSent(userId) {
    return __awaiter(this, void 0, void 0, function* () {
        if (userId === null) {
            return false;
        }
        try {
            const response = yield fetch("/requests/sent/pending", {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                },
            });
            if (!response.ok) {
                throw new Error(`Error: ${response.statusText}`);
            }
            const pendingUsers = yield response.json();
            return pendingUsers.map((user) => user.id).indexOf(userId) != -1;
        } catch (error) {
            console.error("Error unfollowing user:", error);
        }
        return false;
    });
}
