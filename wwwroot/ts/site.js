document.addEventListener("DOMContentLoaded", function () {
    var deletePostButtons = document.querySelectorAll(".delete-post-btn");
    deletePostButtons.forEach(function (deletePostButton) {
        deletePostButton.addEventListener("click", function () {
            var postId = this.getAttribute("data-post-id");
            var parentDiv = this.closest(".post-card");
            fetch("/Post/DeleteConfirmed/?id=".concat(postId), {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "same-origin",
            }).then(function (response) {
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
document.addEventListener("DOMContentLoaded", function () {
    var buttons = document.querySelectorAll(".like-btn");
    buttons.forEach(function (button) {
        button.addEventListener("click", function () {
            var _this = this;
            var postId = this.getAttribute("data-post-id");
            var isLiked = this.getAttribute("data-is-liked") === "True";
            var likeCounter = document.getElementById(
                "like-counter-".concat(postId)
            );
            if (!postId || !likeCounter) {
                alert("Post ID is missing.");
                return;
            }
            fetch(
                "/Post/"
                    .concat(isLiked ? "Unlike" : "Like", "?postId=")
                    .concat(postId),
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    credentials: "same-origin",
                }
            )
                .then(function (response) {
                    if (response.status === 200) {
                        alert("Request successful!");
                        _this.setAttribute(
                            "data-is-liked",
                            !isLiked ? "True" : "False"
                        );
                        _this.innerHTML = !isLiked
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
                .catch(function (error) {
                    console.error("Error toggling like:", error);
                    alert("An error occurred. Please try again.");
                });
        });
    });
});
document.addEventListener("DOMContentLoaded", function () {
    var commentButtons = document.querySelectorAll(".comment-btn");
    commentButtons.forEach(function (button) {
        button.addEventListener("click", function () {
            var postId = this.getAttribute("data-post-id");
            var isOpen = this.getAttribute("data-is-open") === "true";
            if (!postId) {
                alert("Post ID is missing.");
                return;
            }
            var parentDiv = this.closest(".post-card");
            if (!parentDiv) {
                console.error("Could not find the parent post-card div.");
                return;
            }
            var commentInput = parentDiv.querySelector(
                '.comment-input[data-post-id="'.concat(postId, '"]')
            );
            var commentsContainer = parentDiv.querySelector(
                '.comments-container[data-post-id="'.concat(postId, '"]')
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
function createCommentForm(postId) {
    var form = document.createElement("form");
    form.className = "comment-input";
    form.setAttribute("data-post-id", postId);
    form.style.marginTop = "10px";
    form.style.display = "flex";
    form.style.gap = "10px";
    var textarea = document.createElement("textarea");
    textarea.name = "content";
    textarea.placeholder = "Write your comment...";
    textarea.required = true;
    textarea.style.flex = "1";
    textarea.style.padding = "5px";
    textarea.style.border = "1px solid #ccc";
    textarea.style.borderRadius = "5px";
    var submitButton = document.createElement("button");
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
    form.addEventListener("submit", function (event) {
        event.preventDefault();
        var content = textarea.value.trim();
        if (!content) {
            alert("Comment cannot be empty.");
            return;
        }
        submitComment(postId, content, textarea);
    });
    return form;
}
function submitComment(postId, content, textarea) {
    var url = "/Comment/Create?postid="
        .concat(postId, "&content=")
        .concat(content);
    fetch(url, {
        method: "POST",
    })
        .then(function (response) {
            if (response.ok) {
                textarea.value = ""; // Clear the input field
                alert("Comment added successfully!");
                var commentsContainer = document.querySelector(
                    '.comments-container[data-post-id="'.concat(postId, '"]')
                );
                if (commentsContainer) {
                    loadComments(postId, 1, commentsContainer); // Reload comments
                }
            } else {
                throw new Error("Failed to submit comment");
            }
        })
        .catch(function (error) {
            console.error("Error submitting comment:", error);
            alert(
                "An error occurred while submitting your comment. Please try again."
            );
        });
}
function loadComments(postId, pageNumber, container) {
    fetch(
        "/Comment/Index?pageNumber="
            .concat(pageNumber, "&pageSize=5&postId=")
            .concat(postId)
    )
        .then(function (response) {
            if (response.status === 200) {
                return response.json();
            } else {
                throw new Error("Failed to load comments");
            }
        })
        .then(function (comments) {
            if (pageNumber === 1) {
                container.innerHTML = ""; // Clear previous comments
            }
            comments.forEach(function (comment) {
                var commentElement = document.createElement("div");
                commentElement.style.borderBottom = "1px solid #ddd";
                commentElement.style.padding = "5px 0";
                commentElement.innerHTML = "\n                    <p><strong>"
                    .concat(
                        comment.user.userName,
                        "</strong></p>\n                    <p>"
                    )
                    .concat(comment.content, "</p>\n                    <p>")
                    .concat(comment.commentedAt, "</p>\n                ");
                if (comment.isMyComment) {
                    var editButton = document.createElement("a");
                    editButton.setAttribute(
                        "href",
                        "/Comment/Edit?id=" + comment.id
                    );
                    editButton.innerHTML = "Edit";
                    var deleteButton = document.createElement("button");
                    deleteButton.innerText = "Delete";
                    deleteButton.addEventListener("click", function () {
                        fetch("/Comment/Delete?commentId=".concat(comment.id), {
                            method: "POST",
                        })
                            .then(function (response) {
                                if (response.ok) {
                                    alert("Comment deleted successfully!");
                                    loadComments(postId, 1, container); // Reload comments after deletion
                                } else {
                                    throw new Error("Failed to delete comment");
                                }
                            })
                            .catch(function (error) {
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
            var loadMoreButton = container.querySelector(".load-more-btn");
            if (comments.length > 0) {
                if (!loadMoreButton) {
                    var loadMore = document.createElement("button");
                    loadMore.className = "load-more-btn";
                    loadMore.innerText = "Load More";
                    loadMore.style.marginTop = "10px";
                    loadMore.addEventListener("click", function () {
                        return loadComments(postId, pageNumber + 1, container);
                    });
                    container.appendChild(loadMore);
                }
            } else if (loadMoreButton) {
                loadMoreButton.remove();
            }
        })
        .catch(function (error) {
            console.error("Error loading comments:", error);
            alert("An error occurred. Please try again.");
        });
}
