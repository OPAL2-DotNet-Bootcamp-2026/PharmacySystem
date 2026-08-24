document.addEventListener(
    "DOMContentLoaded",
    () => {

        // =====================================
        // CHECK LOGIN
        // =====================================

        if (!Auth.isLoggedIn()) {

            window.location.href =
                "login.html";

            return;
        }


        // =====================================
        // ADMIN ONLY
        // =====================================

        if (
            Auth.role() !== "Admin"
        ) {

            window.location.href =
                "dashboard.html#" +
                Auth.role().toLowerCase();

            return;
        }


        // =====================================
        // ELEMENTS
        // =====================================

        const form =
            document.getElementById(
                "create-user-form"
            );


        const usernameInput =
            document.getElementById(
                "username"
            );


        const emailInput =
            document.getElementById(
                "email"
            );


        const passwordInput =
            document.getElementById(
                "password"
            );


        const roleInput =
            document.getElementById(
                "role"
            );


        const createMessage =
            document.getElementById(
                "create-user-message"
            );


        const usersMessage =
            document.getElementById(
                "users-message"
            );


        const usersSummary =
            document.getElementById(
                "users-summary"
            );


        const usersTableBody =
            document.getElementById(
                "users-table-body"
            );


        const createButton =
            form.querySelector(
                ".btn-create"
            );


        // =====================================
        // LOAD USERS
        // =====================================

        async function loadUsers() {

            usersMessage.textContent =
                "Loading users...";


            try {

                const users =
                    await Api.get(
                        "/User/GetAllUsers"
                    );


                usersTableBody.innerHTML =
                    "";


                users.forEach(
                    user => {

                        const row =
                            document.createElement(
                                "tr"
                            );


                        row.innerHTML = `

                            <td>
                                ${user.username}
                            </td>

                            <td>
                                ${user.email}
                            </td>

                            <td>
                                ${user.role}
                            </td>

                            <td>
                                ${
                                    user.isActive
                                        ? "Active"
                                        : "Inactive"
                                }
                            </td>

                            <td class="text-end">

                                <button
                                    type="button"
                                    class="btn-delete-user"
                                    data-user-id="${user.userID}"
                                >

                                    Delete

                                </button>

                            </td>

                        `;


                        usersTableBody.appendChild(
                            row
                        );

                    }
                );


                usersSummary.textContent =
                    `${users.length} accounts`;


                usersMessage.textContent =
                    "";

            }

            catch (error) {

                usersMessage.textContent =
                    error.message;

            }
        }


        // =====================================
        // CREATE USER
        // =====================================

        form.addEventListener(
            "submit",
            async event => {

                event.preventDefault();


                createMessage.textContent =
                    "";


                const username =
                    usernameInput.value.trim();


                const email =
                    emailInput.value.trim();


                const password =
                    passwordInput.value;


                const role =
                    roleInput.value;


                if (
                    !username ||
                    !email ||
                    !password ||
                    !role
                ) {

                    createMessage.textContent =
                        "Please complete all fields.";

                    return;
                }


                createButton.disabled =
                    true;


                const oldText =
                    createButton.innerHTML;


                createButton.innerHTML =
                    "Creating...";


                try {

                    await Api.post(
                        "/User/create",
                        {
                            username:
                                username,

                            email:
                                email,

                            password:
                                password,

                            role:
                                role
                        }
                    );


                    createMessage.textContent =
                        "User created successfully.";


                    form.reset();


                    await loadUsers();

                }

                catch (error) {

                    createMessage.textContent =
                        error.message;

                }

                finally {

                    createButton.disabled =
                        false;


                    createButton.innerHTML =
                        oldText;

                }

            }
        );


        // =====================================
        // DELETE USER
        // =====================================

        usersTableBody.addEventListener(
            "click",
            async event => {

                const deleteButton =
                    event.target.closest(
                        ".btn-delete-user"
                    );


                if (!deleteButton) {
                    return;
                }


                const userId =
                    deleteButton.dataset.userId;


                const confirmed =
                    confirm(
                        "Are you sure you want to delete this user?"
                    );


                if (!confirmed) {
                    return;
                }


                deleteButton.disabled =
                    true;


                try {

                    await Api.del(
                        `/User/DeleteUser/${userId}`
                    );


                    usersMessage.textContent =
                        "User deleted successfully.";


                    await loadUsers();

                }

                catch (error) {

                    usersMessage.textContent =
                        error.message;


                    deleteButton.disabled =
                        false;

                }

            }
        );


        // =====================================
        // FIRST LOAD
        // =====================================

        loadUsers();

    }
);