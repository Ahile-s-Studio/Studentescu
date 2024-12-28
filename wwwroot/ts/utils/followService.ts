// followService.ts

export async function followUser(userId: string): Promise<void> {
    try {
        const response = await fetch(`/follow/${userId}`, {
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
}

export async function unfollowUser(userId: string): Promise<void> {
    try {
        const response = await fetch(`/unfollow/${userId}`, {
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
}

export async function isFollowingUser(userId: string | null): Promise<boolean> {
    if (userId === null) {
        return false;
    }

    try {
        const response = await fetch("/following", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
            },
        });

        if (!response.ok) {
            throw new Error(`Error: ${response.statusText}`);
        }

        const followingUsers: ApplicationUser[] = await response.json();
        return (
            followingUsers
                .map((user: ApplicationUser) => user.id)
                .indexOf(userId) != -1
        );
    } catch (error) {
        console.error("Error unfollowing user:", error);
    }

    return false;
}

export async function isPendingRequestSent(
    userId: string | null
): Promise<boolean> {
    if (userId === null) {
        return false;
    }

    try {
        const response = await fetch("/requests/sent/pending", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
            },
        });

        if (!response.ok) {
            throw new Error(`Error: ${response.statusText}`);
        }

        const pendingUsers: ApplicationUser[] = await response.json();
        return (
            pendingUsers
                .map((user: ApplicationUser) => user.id)
                .indexOf(userId) != -1
        );
    } catch (error) {
        console.error("Error unfollowing user:", error);
    }

    return false;
}

export interface ApplicationUser {
    firstName: string;
    lastName: string;
    profilePictureUrl: string;
    biography: string;
    public: boolean;
    isProfileCompleted: boolean;
    following: any[];
    posts: any[];
    followers: any[];
    likes: any[];
    comments: any[];
    requestsSent: any[];
    requestsReceived: any[];
    groupMemberships: any[];
    messagesSent: any[];
    privateMessagesReceived: any[];
    id: string;
    userName: string;
    normalizedUserName: string;
    email: string;
    normalizedEmail: string;
    emailConfirmed: boolean;
    passwordHash: string;
    securityStamp: string;
    concurrencyStamp: string;
    phoneNumber: string;
    phoneNumberConfirmed: boolean;
    twoFactorEnabled: boolean;
    lockoutEnd: null;
    lockoutEnabled: boolean;
    accessFailedCount: number;
}
