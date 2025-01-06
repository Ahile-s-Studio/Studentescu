document.addEventListener("DOMContentLoaded", () => {
    attachDeletePostListeners();
    attachLikeButtonListeners();
    attachCommentButtonListeners();

    const observer = new MutationObserver((mutationsList) => {
        for (const mutation of mutationsList) {
            if (
                mutation.type === "childList" &&
                mutation.addedNodes.length > 0
            ) {
                // Reattach event listeners for dynamically added posts
                attachDeletePostListeners();
                attachLikeButtonListeners();
                attachCommentButtonListeners();
            }
        }
    });

    // Start observing the post list for changes
    const postList = document.getElementById("post-list");
    if (postList) {
        observer.observe(postList, {
            childList: true, // Detect when children are added or removed
            subtree: false, // Only observe direct children
        });
    }
});

function attachDeletePostListeners(): void {
    const deletePostButtons =
        document.querySelectorAll<HTMLButtonElement>(".delete-post-btn");

    deletePostButtons.forEach((deletePostButton) => {
        if (deletePostButton.hasAttribute("has-event-listener")) {
            return;
        }
        deletePostButton.setAttribute("has-event-listener", "true");
        deletePostButton.addEventListener("click", function (): void {
            const postId = this.getAttribute("data-post-id");
            const parentDiv = this.closest(".post-card");
            fetch(`/Post/DeleteConfirmed/?id=${postId}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "same-origin",
            }).then((response: Response) => {
                if (response.ok) {
                    alert("Successfully deleted post");
                    if (parentDiv) {
                        parentDiv.remove();
                    }
                }
            });
        });
    });
}

function attachLikeButtonListeners(): void {
    const buttons = document.querySelectorAll<HTMLButtonElement>(".like-btn");

    buttons.forEach((button) => {
        if (button.hasAttribute("has-event-listener")) {
            return;
        }
        button.setAttribute("has-event-listener", "true");
        button.addEventListener("click", function (): void {
            const postId = this.getAttribute("data-post-id");
            const isLiked = this.getAttribute("data-is-liked") === "True";
            const likeCounter = document.getElementById(
                `like-counter-${postId}`
            );

            if (!postId || !likeCounter) {
                alert("Post ID is missing.");
                return;
            }

            fetch(`/Post/${isLiked ? "Unlike" : "Like"}?postId=${postId}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "same-origin",
            })
                .then((response) => {
                    if (response.status === 200) {
                        alert("Request successful!");
                        this.setAttribute(
                            "data-is-liked",
                            !isLiked ? "True" : "False"
                        );
                        this.innerHTML = !isLiked
                            ? "<span>Dislike</span>"
                            : "<span>Like</span>";
                        if (isLiked) {
                            likeCounter.textContent = (
                                parseInt(likeCounter.innerText) - 1
                            ).toString();
                        } else {
                            likeCounter.textContent = (
                                parseInt(likeCounter.innerText) + 1
                            ).toString();
                        }
                    } else {
                        alert("Failed to toggle like. Please try again.");
                    }
                })
                .catch((error) => {
                    console.error("Error toggling like:", error);
                    alert("An error occurred. Please try again.");
                });
        });
    });
}

function attachCommentButtonListeners(): void {
    const commentButtons =
        document.querySelectorAll<HTMLButtonElement>(".comment-btn");

    commentButtons.forEach((button) => {
        if (button.hasAttribute("has-event-listener")) {
            return;
        }
        button.setAttribute("has-event-listener", "true");
        button.addEventListener("click", function (): void {
            const postId = this.getAttribute("data-post-id");
            const isOpen = this.getAttribute("data-is-open") === "true";

            if (!postId) {
                alert("Post ID is missing.");
                return;
            }

            const parentDiv = this.closest(".post-card");
            if (!parentDiv) {
                console.error("Could not find the parent post-card div.");
                return;
            }

            let commentInput = parentDiv.querySelector<HTMLFormElement>(
                `.comment-input[data-post-id="${postId}"]`
            );
            let commentsContainer = parentDiv.querySelector<HTMLDivElement>(
                `.comments-container[data-post-id="${postId}"]`
            );

            if (isOpen) {
                // Close comments section
                if (commentInput) {
                    commentInput.remove();
                }
                if (commentsContainer) {
                    commentsContainer.remove();
                }
                this.setAttribute("data-is-open", "false");
                this.innerText = "Show Comments";
            } else {
                // Open comments section
                if (!commentInput) {
                    commentInput = createCommentForm(postId);
                    parentDiv.appendChild(commentInput);
                }

                if (!commentsContainer) {
                    commentsContainer = document.createElement("div");
                    commentsContainer.classList.add("comments-container");
                    commentsContainer.setAttribute("data-post-id", postId);
                    commentsContainer.style.maxHeight = "500px";
                    commentsContainer.style.overflowY = "scroll";
                    commentsContainer.style.border = "1px solid #ccc";
                    commentsContainer.style.padding = "10px";
                    commentsContainer.style.marginTop = "10px";

                    parentDiv.appendChild(commentsContainer);
                }

                loadComments(postId, 1, commentsContainer);

                this.setAttribute("data-is-open", "true");
                this.innerText = "Hide Comments";
            }
        });
    });
}
function createCommentForm(postId: string): HTMLFormElement {
    const form = document.createElement("form");
    form.className = "comment-input";
    form.setAttribute("data-post-id", postId);
    form.style.marginTop = "10px";
    form.style.display = "flex";
    form.style.gap = "10px";
    form.style.alignItems = "center";

    const textarea = document.createElement("textarea");
    textarea.name = "content";
    textarea.placeholder = "Write your comment...";
    textarea.required = true;
    textarea.style.flex = "1";
    textarea.style.padding = "10px";
    textarea.style.border = "1px solid white";
    textarea.style.borderRadius = "5px";
    textarea.style.backgroundColor = "transparent";
    textarea.style.color = "white";
    textarea.style.fontSize = "14px";
    textarea.style.outline = "none";

    const submitButton = document.createElement("button");
    submitButton.type = "submit";
    submitButton.innerText = "Comment";
    submitButton.style.padding = "8px 16px";
    submitButton.style.border = "none";
    submitButton.style.backgroundColor =
        "rgb(168 85 247 / var(--tw-bg-opacity, 1))"; // Correct purple color

    submitButton.style.color = "white";
    submitButton.style.borderRadius = "5px";
    submitButton.style.cursor = "pointer";
    submitButton.style.fontSize = "14px";

    form.appendChild(textarea);
    form.appendChild(submitButton);

    form.addEventListener("submit", (event) => {
        event.preventDefault();

        const content = textarea.value.trim();
        if (!content) {
            alert("Comment cannot be empty.");
            return;
        }

        submitComment(postId, content, textarea);
    });

    return form;
}

function submitComment(
    postId: string,
    content: string,
    textarea: HTMLTextAreaElement
): void {
    const url = `/Comment/Create?postid=${postId}&content=${content}`;

    fetch(url, {
        method: "POST",
    })
        .then((response) => {
            if (response.ok) {
                textarea.value = ""; // Clear the input field
                alert("Comment added successfully!");
                const commentsContainer =
                    document.querySelector<HTMLDivElement>(
                        `.comments-container[data-post-id="${postId}"]`
                    );
                if (commentsContainer) {
                    loadComments(postId, 1, commentsContainer); // Reload comments
                }
            } else {
                throw new Error("Failed to submit comment");
            }
        })
        .catch((error) => {
            console.error("Error submitting comment:", error);
            alert(
                "An error occurred while submitting your comment. Please try again."
            );
        });
}
function loadComments(
    postId: string,
    pageNumber: number,
    container: HTMLElement
): void {
    fetch(`/Comment/Index?pageNumber=${pageNumber}&pageSize=5&postId=${postId}`)
        .then((response) => {
            if (response.status === 200) {
                return response.json();
            } else {
                throw new Error("Failed to load comments");
            }
        })
        .then((comments: Comment[]) => {
            if (pageNumber === 1) {
                container.innerHTML = ""; // Clear previous comments
            }

            comments.forEach((comment) => {
                const commentElement = document.createElement("div");
                commentElement.className = "comment-item";
                commentElement.style.borderBottom = "1px solid #ddd";
                commentElement.style.padding = "15px 0";
                commentElement.style.marginBottom = "10px";

                commentElement.innerHTML = `
                    <p class="comment-author text-white"><strong>${comment.user.userName}</strong></p>
                    <p class="comment-content text-white">${comment.content}</p>
                    <p class="comment-time text-gray-400">${comment.commentedAt}</p>
                `;

                if (comment.isMyComment) {
                    const editButton = document.createElement("a");
                    editButton.setAttribute(
                        "href",
                        "/Comment/Edit?id=" + comment.id
                    );
                    editButton.innerHTML = "Edit";
                    editButton.style.marginRight = "10px";
                    editButton.style.color =
                        "rgb(168 85 247 / var(--tw-bg-opacity, 1))";
                    editButton.style.textDecoration = "none";

                    const deleteButton = document.createElement("button");
                    deleteButton.innerText = "Delete";
                    deleteButton.style.backgroundColor = "rgb(220 38 38)"; // Red for delete
                    deleteButton.style.color = "white";
                    deleteButton.style.padding = "5px 10px";
                    deleteButton.style.border = "none";
                    deleteButton.style.borderRadius = "5px";
                    deleteButton.style.cursor = "pointer";
                    deleteButton.addEventListener("click", () => {
                        fetch(`/Comment/Delete?commentId=${comment.id}`, {
                            method: "POST",
                        })
                            .then((response) => {
                                if (response.ok) {
                                    alert("Comment deleted successfully!");
                                    loadComments(postId, 1, container); // Reload comments after deletion
                                } else {
                                    throw new Error("Failed to delete comment");
                                }
                            })
                            .catch((error) => {
                                console.error("Error deleting comment:", error);
                                alert(
                                    "An error occurred while deleting the comment. Please try again."
                                );
                            });
                    });

                    commentElement.appendChild(editButton);
                    commentElement.appendChild(deleteButton);
                }

                container.appendChild(commentElement);
            });

            const loadMoreButton =
                container.querySelector<HTMLButtonElement>(".load-more-btn");
            if (comments.length > 0) {
                if (!loadMoreButton) {
                    const loadMore = document.createElement("button");
                    loadMore.innerText = "Load more...";
                    loadMore.classList.add("load-more-btn");
                    loadMore.style.backgroundColor =
                        "rgb(168 85 247 / var(--tw-bg-opacity, 1))"; // Purple button
                    loadMore.style.color = "white";
                    loadMore.style.padding = "8px 16px";
                    loadMore.style.border = "none";
                    loadMore.style.borderRadius = "5px";
                    loadMore.style.cursor = "pointer";
                    loadMore.addEventListener("click", () => {
                        loadComments(postId, pageNumber + 1, container);
                    });
                    container.appendChild(loadMore);
                }
            } else {
                if (loadMoreButton) {
                    loadMoreButton.remove();
                }
            }
        })
        .catch((error) => {
            console.error("Error loading comments:", error);
            alert(
                "An error occurred while loading comments. Please try again."
            );
        });
}

interface Comment {
    id: number;
    content: string;
    commentedAt: string; // Use `Date` if you're parsing it into a Date object
    user: {
        id: string;
        userName: string;
        profilePictureUrl: string | null;
    };
    isMyComment: boolean;
}
