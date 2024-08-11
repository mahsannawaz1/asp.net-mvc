function getQueryParam(param) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(param);
}

// Get the role from the query parameters
const role = getQueryParam('role');
console.log(role);

document.addEventListener('DOMContentLoaded', function () {
    const formElement = document.querySelector('.signup-form');
    console.log(formElement);

    // Modify the form action URL dynamically
    if (formElement) {
        formElement.addEventListener('submit', function (event) {
            this.action = `/User/SignUp?role=${encodeURIComponent(role)}`;
        });
    }
});