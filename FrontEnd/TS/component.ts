fetch("../components/footer.html")
.then(response=>response.text())
.then((data:string)=>{
    const footerContainer=document.getElementById("footer-container");

    if(footerContainer){
        footerContainer.innerHTML=data;
    }
})
.catch((error:unknown)=>{
    console.error("Error loading footer:",error);
});