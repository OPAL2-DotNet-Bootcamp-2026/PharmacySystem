const Auth = {
    save(data) {
        localStorage.setItem("token", data.token);
        localStorage.setItem("username", data.username);
        localStorage.setItem("role", data.role);
    },
    token() {return localStorage.getItem("token"); },
        
   
    role() { return localStorage.getItem("role"); },
       
   
    isLoggedIn() {return !!this.token();},
        
    
    logout() {
        localStorage.clear();
        window.location.href = "login.html";
    }
};