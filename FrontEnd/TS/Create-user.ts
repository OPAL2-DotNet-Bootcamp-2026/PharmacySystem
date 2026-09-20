// =====================================
// TYPES
// =====================================

interface User {
    userID: number;
    username: string;
    email: string;
    role: UserRole;
    isActive: boolean;
}

interface Pharmacy {
    pharmacyID: number;
    pharmacyName: string;
}

interface PharmacistCreateRequest {
    username: string;
    fullName: string;
    phone: string;
    email: string;
    password: string;
    pharmacyID: number;
}

interface UserCreateRequest {
    username: string;
    email: string;
    password: string;
    role: UserRole;
}



// PAGE LOAD

document.addEventListener(
    "DOMContentLoaded",
    () => {


        // CHECK LOGIN

        if (!Auth.isLoggedIn()) {

            window.location.href ="login.html";
                

            return;
        }


        // ADMIN ONLY

        const currentRole =Auth.role();
            


        if (
            currentRole !== "Admin"
        ) {

            window.location.href =
                "dashboard.html#"
                +
                (currentRole ?? "").toLowerCase();

            return;
        }


        // ELEMENTS

        const form =
            document.getElementById(
                "create-user-form"
            ) as HTMLFormElement;


        const usernameInput =
            document.getElementById(
                "username"
            ) as HTMLInputElement;


        const emailInput =
            document.getElementById(
                "email"
            ) as HTMLInputElement;


        const passwordInput =
            document.getElementById(
                "password"
            ) as HTMLInputElement;


        const roleInput =
            document.getElementById(
                "role"
            ) as HTMLSelectElement;


        const pharmacistFields =
            document.getElementById(
                "pharmacist-fields"
            ) as HTMLElement;


        const fullNameInput =
            document.getElementById(
                "fullName"
            ) as HTMLInputElement;


        const phoneInput =
            document.getElementById(
                "phone"
            ) as HTMLInputElement;


        const pharmacyInput =
            document.getElementById(
                "pharmacyId"
            ) as HTMLSelectElement;


        const createMessage =
            document.getElementById(
                "create-user-message"
            ) as HTMLElement;


        const usersMessage =
            document.getElementById(
                "users-message"
            ) as HTMLElement;


        const usersSummary =
            document.getElementById(
                "users-summary"
            ) as HTMLElement;


        const usersTableBody =
            document.getElementById(
                "users-table-body"
            ) as HTMLTableSectionElement;


        const createButton =
            form.querySelector(
                ".btn-create"
            ) as HTMLButtonElement;


        // VALIDATION PATTERNS

        const emailPattern: RegExp = /^[^@\s]+@[^@\s]+\.com$/;
           


        const phonePattern: RegExp = /^\+968 [0-9]{8}$/;
           


        // LOAD USERS

        async function loadUsers(): Promise<void> {

            usersMessage.textContent =
                "Loading users...";


            try {

                const users =
                    await Api.get<User[]>(
                        "/User/GetAllUsers"
                    );


                usersTableBody.innerHTML = "";
                   


                users.forEach(
                    (user: User) => {


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


                usersSummary.textContent =`${users.length} accounts`;
                    


                usersMessage.textContent = "";
                   

            }

            catch (error) {

                if (error instanceof Error) {

                    usersMessage.textContent = error.message;
                       

                }

            }

        }


        // LOAD PHARMACIES

        async function loadPharmacies(): Promise<void> {

            try {

                const pharmacies =
                    await Api.get<Pharmacy[]>(
                        "/Pharmacy"
                    );


                pharmacyInput.innerHTML = `

                    <option value="">
                        Select pharmacy
                    </option>

                `;


                pharmacies.forEach(
                    (pharmacy: Pharmacy) => {


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

                console.error("Failed to load pharmacies:", error );
                    


                pharmacyInput.innerHTML = `

                    <option value="">
                        Could not load pharmacies
                    </option>

                `;

            }

        }


        // ROLE CHANGE

        roleInput.addEventListener(
            "change",
            () => {


                if (
                    roleInput.value ==="Pharmacist"
                    
                ) {

                    pharmacistFields.style.display ="block";
                        
                    fullNameInput.required =true;
                        
                    phoneInput.required =true;
   
                    pharmacyInput.required = true;
                       

                }

                else {

                    pharmacistFields.style.display ="none";
    
                    fullNameInput.required =false;
      
                    phoneInput.required = false;

                    pharmacyInput.required =false;    

                    fullNameInput.value ="";
     
                    phoneInput.value ="";
                        
                    pharmacyInput.value = "";
                       

                }

            }
        );


        // CREATE USER

        form.addEventListener(
            "submit",
            async (event: SubmitEvent) => {


                event.preventDefault();


                createMessage.textContent = "";

                   

                // GET BASIC VALUES

                const username: string =usernameInput.value.trim();
      
                const email: string =emailInput.value.trim();

                const password: string =passwordInput.value;
                    
                const role =roleInput.value as UserRole;
                    


                // USERNAME VALIDATION

                if (!username) {

                    createMessage.textContent =
                        "Username is required.";

                    usernameInput.focus();

                    return;
                }


                if (
                    username.length > 50
                ) {

                    createMessage.textContent ="Username cannot be more than 50 characters.";
                        
                    usernameInput.focus();

                    return;
                }


                
                // EMAIL VALIDATION

                if (!email) {

                    createMessage.textContent ="Email is required.";
                        
                    emailInput.focus();

                    return;
                }


                if (
                    email.length > 100
                ) {

                    createMessage.textContent = "Email cannot be more than 100 characters.";
                       
                    emailInput.focus();

                    return;
                }


                if (
                    !emailPattern.test( email)
                       
                    
                ) {

                    createMessage.textContent ="Email must contain @ and end with .com";
                        
                    emailInput.focus();

                    return;
                }


                // PASSWORD VALIDATION

                if (!password) {

                    createMessage.textContent ="Password is required.";
                        

                    passwordInput.focus();

                    return;
                }


                if (
                    password.length < 8
                ) {

                    createMessage.textContent ="Password must be at least 8 characters.";
                        

                    passwordInput.focus();

                    return;
                }


                // ROLE VALIDATION

                if (!roleInput.value) {

                    createMessage.textContent ="Please choose a role.";    

                    roleInput.focus();

                    return;
                }


                
                // PHARMACIST VALUES

                let fullName: string | null = null;
                   


                let phone: string | null = null;
                   


                let pharmacyID: number | null = null;
                   


                // PHARMACIST VALIDATION

                if (
                    role === "Pharmacist"
                ) {

                    fullName =
                        fullNameInput
                            .value
                            .trim();


                    phone =
                        phoneInput
                            .value
                            .trim();


                    pharmacyID = Number(pharmacyInput.value );
                       
                                  

                    // FULL NAME

                    if (!fullName) {

                        createMessage.textContent ="Full name is required.";
                            

                        fullNameInput.focus();

                        return;
                    }


                    if (
                        fullName.length > 100
                    ) {

                        createMessage.textContent ="Full name cannot be more than 100 characters.";
                            
                        fullNameInput.focus();

                        return;
                    }


                    // PHONE

                    if (!phone) {

                        createMessage.textContent ="Phone number is required.";
                            
                        phoneInput.focus();

                        return;
                    }


                    if (
                        !phonePattern.test( phone)
                           
                        
                    ) {

                        createMessage.textContent = "Phone must be in the format +968 99112233";
                           

                        phoneInput.focus();

                        return;
                    }


                    // PHARMACY

                    if (
                        !pharmacyID
                        ||
                        pharmacyID < 1
                    ) {

                        createMessage.textContent ="Please select a valid pharmacy.";
                            
                        pharmacyInput.focus();

                        return;
                    }

                }


                // START CREATING USER

                createButton.disabled =true;
           

                const oldText: string =createButton.innerHTML;              

                createButton.innerHTML ="Creating...";
                    
                try {


                    // CREATE PHARMACIST

                    if (
                        role === "Pharmacist"
                        &&
                        fullName
                        &&
                        phone
                        &&
                        pharmacyID
                    ) {

                        const pharmacistData:
                            PharmacistCreateRequest = {

                                username: username,
                                fullName: fullName,
                                phone: phone,                             
                                email:email,
                                password:password, 
                                pharmacyID:pharmacyID
                                    
                            };


                        await Api.post<
                            unknown,
                            PharmacistCreateRequest
                        >(
                            "/Pharmacist",
                            pharmacistData
                        );

                    }


                    // CREATE ADMIN / MANAGER

                    else {

                        const userData:
                            UserCreateRequest = {

                                username:  username,
                                email:email,
                                password:password,                           
                                role: role
                                   
                            };


                        await Api.post<
                            unknown,
                            UserCreateRequest
                        >(
                            "/User/create",
                            userData
                        );

                    }


                    // SUCCESS

                    createMessage.textContent ="User created successfully.";
                        


                    form.reset();

                    pharmacistFields.style.display ="none";                
                    fullNameInput.required =false;
                    phoneInput.required =false;
                    pharmacyInput.required =false;
                        


                    await loadUsers();

                }

                catch (error) {

                    if (error instanceof Error) {

                        createMessage.textContent =error.message;
                        
                    }

                }

                finally {

                    createButton.disabled =false;
                    createButton.innerHTML =oldText;
                        

                }

            }
        );


        // CLEAR BUTTON

        form.addEventListener(
            "reset",
            () => {


                pharmacistFields.style.display = "none";       
                fullNameInput.required =false;
                phoneInput.required =false;
                pharmacyInput.required =false;  
                createMessage.textContent = "";
                   

            }
        );


        // DELETE USER

        usersTableBody.addEventListener(
            "click",
            async (event: MouseEvent) => {


                const target =
                    event.target as HTMLElement;


                const deleteButton =
                    target.closest(
                        ".btn-delete-user"
                    ) as HTMLButtonElement | null;


                if (!deleteButton) {

                    return;
                }


                const userId: string | undefined =deleteButton.dataset.userId;
                    


                if (!userId) {

                    return;
                }


                const confirmed: boolean =confirm("Are you sure you want to delete this user?");
                    
            
                if (!confirmed) {

                    return;
                }


                deleteButton.disabled =true;
                    
                try {

                    await Api.del<unknown>(

                        `/User/DeleteUser/${userId}`

                    );
                    usersMessage.textContent ="User deleted successfully.";


                    await loadUsers();

                }

                catch (error) {

                    if (error instanceof Error) {

                        usersMessage.textContent =error.message;
                            

                    }

                    deleteButton.disabled =false;

                        
                }

            }
        );


        // FIRST LOAD

        loadUsers();

        loadPharmacies();

    }
);