"use strict";
const Auth = {
    // SAVE USER DATA
    save(data) {
        localStorage.setItem("token", data.token);
        localStorage.setItem("username", data.username);
        localStorage.setItem("role", data.role);
    },
    // GET TOKEN
    token() {
        return localStorage.getItem("token");
    },
    // GET ROLE
    role() {
        return localStorage.getItem("role");
    },
    // CHECK LOGIN
    isLoggedIn() {
        return !!this.token();
    },
    // LOGOUT
    logout() {
        localStorage.clear();
        window.location.href = "login.html";
    }
};
