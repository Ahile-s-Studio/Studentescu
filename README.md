# Studentescu

Studentescu is a social media platform designed specifically for students, enabling them to connect, share, and collaborate. Users can post text, upload images, and share YouTube videos while interacting through comments and appreciating posts.

The platform also features groups where students with common interests can join, share content, and build connections. To maintain a safe and positive environment, an admin moderates the platform, ensuring that inappropriate content is promptly removed.

StudentConnect fosters collaboration and creativity, offering students a space to engage with peers, explore interests, and grow together.

## Getting started

To get started with the development environment for **Studentescu**, follow these steps to set up the application using Docker Compose:

---

### Prerequisites

Make sure you have the following installed on your system:

- **Docker**: [Install Docker](https://docs.docker.com/get-docker/)
- **Docker Compose**: [Install Docker Compose](https://docs.docker.com/compose/install/)

---

### Steps to Set Up

1. **Clone the Repository**

    ```bash
    git clone https://github.com/Ahile-s-Studio/Studentescu.git
    cd studentescu
    ```

2. **Start the Application**

    Use Docker Compose to build and start the containers:

    ```bash
    docker-compose up
    ```

### Alternative: TMUX Shell Script for Managing the Development Environment

1.  **Run the Script**
    Start the TMUX session and Docker environment with:

        ```bash
        ./start_studentescu.sh
        ```

2.  **Session Details**

    - **Window** 1 (asp.net): Runs the asp.net application in hot reload mode.
    - **Window** 2 (mysql): Runs the mysql database.
    - **Window** 3 (tsc & tailwind compilers): Runs the npm run watch:dev script.

3.  **Access the Application**

    The application will be accessible at http://localhost:5000 and the mysql database will be accessible at http://localhost:3306.

## Pull Request (PR) Guidelines

Follow these rules to ensure consistency and quality when contributing to the repository:

### 1. Branch Naming Convention

- Create a new branch for each task or issue you are working on.
- Name the branch following this pattern:  
  **`#issueNumber-issueName`**  
  Examples:
    - For issue #45 about fixing login bugs: `#45-fix-login-bug`
    - For issue #12 about adding a new feature: `#12-add-user-profile`

---

### 2. Scope of Pull Requests

- **Single Issue or Related Issues Only**:  
  Each pull request should resolve one issue or multiple directly related issues. Avoid including unrelated changes in a single PR.  
  Examples:
    - ✅ Resolves issue #23: "Fix button alignment"
    - ✅ Resolves issues #30 and #31: "Update form design" and "Add accessibility labels" (because they are related).
    - ❌ Do not include fixes for "form design" and "authentication logic" in one PR.

---

### 3. Testing Before Submission

- Test your changes thoroughly **before** creating a pull request:
    - Use the provided **Docker environment** and test the application using the scripts in the "Get Started" section.
    - Run all automated tests and ensure they pass.
    - Manually test critical functionality affected by your changes.

---

### 4. Commit Messages

- Write clear and descriptive commit messages.  
  Examples:
    - ✅ `Fix alignment issue in login form`
    - ✅ `Add Docker configuration for backend testing`
    - ❌ `Fixed stuff`

---

### 5. Pull Request Title and Description

- Use a clear, descriptive title for your PR.  
  Examples:
    - ✅ `#45 Fix button alignment issue on login page`
    - ✅ `#12 Add user profile page`
- Include the following in the PR description:
    - A summary of the changes made.
    - The issue(s) it resolves (e.g., "Resolves #12").
    - Steps to test the changes locally, if applicable.

---

### 6. Code Review Checklist

Before submitting your pull request, ensure:

- Your code adheres to the repository's coding standards.
- There are no unused variables, unnecessary comments, or debugging lines (e.g., `console.log` or `System.out.println`).
- All changes are necessary and related to the issue.
- Documentation is updated (if applicable).

---

### 7. Resolving Conflicts

- Pull the latest changes from the `main` branch (or the base branch) into your feature branch.
- Resolve any merge conflicts **before** creating the pull request.

---

### 8. CI/CD Pipeline

- Ensure your changes do not break the Continuous Integration/Continuous Deployment (CI/CD) pipeline.
- Fix any pipeline errors before requesting a review.

---

### 9. Review Request

- Once your pull request is ready:
    - Assign it to a reviewer or notify the team.
    - Be responsive to feedback and make updates promptly.

---

### 10. Post-Merge Cleanup

- After your PR is approved and merged, delete your feature branch to keep the repository clean.

---

By following these guidelines, you help ensure that the repository remains organized, and contributions maintain a high standard of quality. Thank you for your cooperation!

---

## Milestones

The entire project is split in 3 milestones.

#### 1. Alpha Release

- Intended to achieve the basic functionalities of a social media platform
    - Support authentification and creating profiles
    - The possibility to create posts
    - Following other users
    - Editing your profile
- Support a consistent design direction
- Make sure everything is secure and no one can make illegal request or actions

#### 2. Beta Release

- Support the possibiity of private profiles
- Creating groups and manage them
- Notifications related to requests of follow and join groups
- Support comments under posts

#### 3. Enhencements

- Video/Voicecalls
- Algorithm for recommendations

---

## Our Future Direction

We are committed to continuously improving and evolving this project to better serve our users. Here are some of the future updates and features we plan to implement:

1. **Platform-Specific Enhancements**

    - Introducing features tailored to modern and popular platforms to ensure seamless integration and usability across devices.

2. **Student-Centric Tools**

    - Providing students with practical features like:
        - **Study Groups**: Tools to connect with peers for collaborative learning and shared goals.
        - **Social Opportunities**: Creating spaces for students to hang out, build friendships, and enhance their university experience.

3. **Focus on Student Life**

    - Expanding the platform to address more aspects of student life, such as:
        - Event planning and participation.
        - Resource-sharing platforms for books, notes, or tutorials.
        - Time management tools to help students stay organized.

4. **Community Feedback and Iterative Development**
    - Actively listening to user feedback to prioritize and shape future updates.
    - Regularly iterating on existing features to make them more intuitive and valuable.

This vision ensures that we remain dedicated to empowering students and improving their academic and social experiences through modern, thoughtful, and innovative solutions. Stay tuned for exciting updates!
