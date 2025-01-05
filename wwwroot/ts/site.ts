document.addEventListener("DOMContentLoaded", () => {
    const deletePostButtons =
        document.querySelectorAll<HTMLButtonElement>(".delete-post-btn");

    deletePostButtons.forEach((deletePostButton) => {
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
});
document.addEventListener("DOMContentLoaded", () => {
    const buttons = document.querySelectorAll<HTMLButtonElement>(".like-btn");

    buttons.forEach((button) => {
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
});

document.addEventListener("DOMContentLoaded", () => {
    const commentButtons =
        document.querySelectorAll<HTMLButtonElement>(".comment-btn");

    commentButtons.forEach((button) => {
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
});

function createCommentForm(postId: string): HTMLFormElement {
    const form = document.createElement("form");
    form.className = "comment-input";
    form.setAttribute("data-post-id", postId);
    form.style.marginTop = "10px";
    form.style.display = "flex";
    form.style.gap = "10px";

    const textarea = document.createElement("textarea");
    textarea.name = "content";
    textarea.placeholder = "Write your comment...";
    textarea.required = true;
    textarea.style.flex = "1";
    textarea.style.padding = "5px";
    textarea.style.border = "1px solid #ccc";
    textarea.style.borderRadius = "5px";

    const submitButton = document.createElement("button");
    submitButton.type = "submit";
    submitButton.innerText = "Comment";
    submitButton.style.padding = "5px 10px";
    submitButton.style.border = "none";
    submitButton.style.backgroundColor = "#007bff";
    submitButton.style.color = "#fff";
    submitButton.style.borderRadius = "5px";
    submitButton.style.cursor = "pointer";

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
                commentElement.style.borderBottom = "1px solid #ddd";
                commentElement.style.padding = "5px 0";
                commentElement.innerHTML = `
                    <p><strong>${comment.user.userName}</strong></p>
                    <p>${comment.content}</p>
                    <p>${comment.commentedAt}</p>
                `;
                if (comment.isMyComment) {
                    const editButton = document.createElement("a");
                    editButton.setAttribute(
                        "href",
                        "/Comment/Edit?id=" + comment.id
                    );
                    editButton.innerHTML = "Edit";
                    const deleteButton = document.createElement("button");
                    deleteButton.innerText = "Delete";
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
                    loadMore.className = "load-more-btn";
                    loadMore.innerText = "Load More";
                    loadMore.style.marginTop = "10px";
                    loadMore.addEventListener("click", () =>
                        loadComments(postId, pageNumber + 1, container)
                    );
                    container.appendChild(loadMore);
                }
            } else if (loadMoreButton) {
                loadMoreButton.remove();
            }
        })
        .catch((error) => {
            console.error("Error loading comments:", error);
            alert("An error occurred. Please try again.");
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
