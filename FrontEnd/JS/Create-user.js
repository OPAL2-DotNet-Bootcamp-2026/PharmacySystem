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
                "dashboard.html#"
                +
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


        const pharmacistFields =
            document.getElementById(
                "pharmacist-fields"
            );


        const fullNameInput =
            document.getElementById(
                "fullName"
            );


        const phoneInput =
            document.getElementById(
                "phone"
            );


        const pharmacyInput =
            document.getElementById(
                "pharmacyId"
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
        // VALIDATION PATTERNS
        // =====================================

        const emailPattern =
            /^[^@\s]+@[^@\s]+\.com$/;


        const phonePattern =
            /^\+968 [0-9]{8}$/;



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
                                        ?
                                        "Active"
                                        :
                                        "Inactive"
                                }

                            </td>


                            <td class="text-end">


                             <button
                                        type="button"
                                        class="btn-delete-user"
                                        data-user-id="${user.userID}"
                                        title="Delete user"
                            >

                            <i class="bi bi-trash3"></i>

                             <span>
                                    Delete
                             </span>

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
        // LOAD PHARMACIES
        // =====================================

        async function loadPharmacies() {


            try {


                const pharmacies =
                    await Api.get(
                        "/Pharmacy"
                    );


                pharmacyInput.innerHTML = `

                    <option value="">

                        Select pharmacy

                    </option>

                `;


                pharmacies.forEach(
                    pharmacy => {


                        pharmacyInput.innerHTML += `

                            <option
                                value="${pharmacy.pharmacyID}"
                            >

                                ${pharmacy.pharmacyName}

                            </option>

                        `;

                    }
                );

            }


            catch (error) {


                console.error(

                    "Failed to load pharmacies:",

                    error

                );


                pharmacyInput.innerHTML = `

                    <option value="">

                        Could not load pharmacies

                    </option>

                `;

            }

        }



        // =====================================
        // ROLE CHANGE
        // =====================================

        roleInput.addEventListener(
            "change",
            () => {


                // =================================
                // PHARMACIST
                // =================================

                if (
                    roleInput.value ===
                    "Pharmacist"
                ) {


                    pharmacistFields.style.display =
                        "block";


                    // Full name required

                    fullNameInput.required =
                        true;


                    // Phone required

                    phoneInput.required =
                        true;


                    // Pharmacy required

                    pharmacyInput.required =
                        true;


                }


                // =================================
                // ADMIN / MANAGER
                // =================================

                else {


                    pharmacistFields.style.display =
                        "none";


                    fullNameInput.required =
                        false;


                    phoneInput.required =
                        false;


                    pharmacyInput.required =
                        false;


                    fullNameInput.value =
                        "";


                    phoneInput.value =
                        "";


                    pharmacyInput.value =
                        "";

                }

            }
        );



        // =====================================
        // CREATE USER
        // =====================================

        form.addEventListener(
            "submit",
            async event => {


                event.preventDefault();


                createMessage.textContent =
                    "";



                // =================================
                // GET BASIC VALUES
                // =================================

                const username =
                    usernameInput.value.trim();


                const email =
                    emailInput.value.trim();


                const password =
                    passwordInput.value;


                const role =
                    roleInput.value;



                // =================================
                // USERNAME VALIDATION
                // =================================

                if (!username) {


                    createMessage.textContent =
                        "Username is required.";


                    usernameInput.focus();


                    return;

                }


                if (
                    username.length > 50
                ) {


                    createMessage.textContent =
                        "Username cannot be more than 50 characters.";


                    usernameInput.focus();


                    return;

                }



                // =================================
                // EMAIL VALIDATION
                // =================================

                if (!email) {


                    createMessage.textContent =
                        "Email is required.";


                    emailInput.focus();


                    return;

                }


                if (
                    email.length > 100
                ) {


                    createMessage.textContent =
                        "Email cannot be more than 100 characters.";


                    emailInput.focus();


                    return;

                }


                if (
                    !emailPattern.test(
                        email
                    )
                ) {


                    createMessage.textContent =
                        "Email must contain @ and end with .com";


                    emailInput.focus();


                    return;

                }



                // =================================
                // PASSWORD VALIDATION
                // =================================

                if (!password) {


                    createMessage.textContent =
                        "Password is required.";


                    passwordInput.focus();


                    return;

                }


                if (
                    password.length < 8
                ) {


                    createMessage.textContent =
                        "Password must be at least 8 characters.";


                    passwordInput.focus();


                    return;

                }



                // =================================
                // ROLE VALIDATION
                // =================================

                if (!role) {


                    createMessage.textContent =
                        "Please choose a role.";


                    roleInput.focus();


                    return;

                }



                // =================================
                // PHARMACIST VALUES
                // =================================

                let fullName =
                    null;


                let phone =
                    null;


                let pharmacyID =
                    null;



                // =================================
                // PHARMACIST VALIDATION
                // =================================

                if (
                    role ===
                    "Pharmacist"
                ) {


                    fullName =
                        fullNameInput
                            .value
                            .trim();


                    phone =
                        phoneInput
                            .value
                            .trim();


                    pharmacyID =
                        Number(
                            pharmacyInput.value
                        );



                    // FULL NAME

                    if (!fullName) {


                        createMessage.textContent =
                            "Full name is required.";


                        fullNameInput.focus();


                        return;

                    }


                    if (
                        fullName.length > 100
                    ) {


                        createMessage.textContent =
                            "Full name cannot be more than 100 characters.";


                        fullNameInput.focus();


                        return;

                    }



                    // PHONE

                    if (!phone) {


                        createMessage.textContent =
                            "Phone number is required.";


                        phoneInput.focus();


                        return;

                    }


                    if (
                        !phonePattern.test(
                            phone
                        )
                    ) {


                        createMessage.textContent =
                            "Phone must be in the format +968 99112233";


                        phoneInput.focus();


                        return;

                    }



                    // PHARMACY

                    if (
                        !pharmacyID
                        ||
                        pharmacyID < 1
                    ) {


                        createMessage.textContent =
                            "Please select a valid pharmacy.";


                        pharmacyInput.focus();


                        return;

                    }

                }



                // =================================
                // START CREATING USER
                // =================================

                createButton.disabled =
                    true;


                const oldText =
                    createButton.innerHTML;


                createButton.innerHTML =
                    "Creating...";



                try {


                    // =================================
                    // CREATE PHARMACIST
                    // =================================

                    if (
                        role ===
                        "Pharmacist"
                    ) {


                        await Api.post(
                            "/Pharmacist",
                            {


                                username:
                                    username,


                                fullName:
                                    fullName,


                                phone:
                                    phone,


                                email:
                                    email,


                                password:
                                    password,


                                pharmacyID:
                                    pharmacyID


                            }
                        );

                    }


                    // =================================
                    // CREATE ADMIN / MANAGER
                    // =================================

                    else {


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

                    }



                    // =================================
                    // SUCCESS
                    // =================================

                    createMessage.textContent =
                        "User created successfully.";


                    form.reset();


                    pharmacistFields.style.display =
                        "none";


                    fullNameInput.required =
                        false;


                    phoneInput.required =
                        false;


                    pharmacyInput.required =
                        false;


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
        // CLEAR BUTTON
        // =====================================

        form.addEventListener(
            "reset",
            () => {


                pharmacistFields.style.display =
                    "none";


                fullNameInput.required =
                    false;


                phoneInput.required =
                    false;


                pharmacyInput.required =
                    false;


                createMessage.textContent =
                    "";

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


        loadPharmacies();


    }
);