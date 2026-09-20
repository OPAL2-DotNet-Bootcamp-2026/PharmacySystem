"use strict";
fetch("../components/footer.html")
    .then(response => response.text())
    .then((data) => {
    const footerContainer = document.getElementById("footer-container");
    if (footerContainer) {
        footerContainer.innerHTML = data;
    }
})
    .catch((error) => {
    console.error("Error loading footer:", error);
});
