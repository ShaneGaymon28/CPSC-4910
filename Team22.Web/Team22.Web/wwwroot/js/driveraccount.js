const form = document.getElementById('form');

const ogFirstName = document.getElementById('ogFirstName');
const ogLastName = document.getElementById('ogLastName');
const ogEmail = document.getElementById('ogEmail');
const ogUserName = document.getElementById('ogUserName');

const firstName = document.getElementById('first-name');
const lastName = document.getElementById('last-name');
const email = document.getElementById('email');
const userRole = document.getElementById('userRole');
const userName = document.getElementById('userName');

setWarningFor(userRole, 'UserRole is read-only.');
setWarningFor(userName, 'UserName is read-only.');


form.addEventListener('submit', e => {
    e.preventDefault();

    if(checkInputs()){
        document.getElementById('form').submit();
    }
});


function checkInputs() {
    const firstNameValue = firstName.value.trim();
    const lastNameValue = lastName.value.trim();
    const emailValue = email.value.trim();
    
    const ogFNValue = ogFirstName.value.trim();
    const ogLNValue = ogLastName.value.trim();
    const ogEmailValue = ogEmail.value.trim();
    
    let falseFlag = false;

    if(firstNameValue === ogFNValue) {
        setWarningFor(firstName, 'No change made to first name.');
    }
    else {
        setSuccessFor(firstName, 'First name changed.');
    }

    if(lastNameValue === ogLNValue) {
        setWarningFor(lastName, 'No change made to last name.');
    }
    else {
        setSuccessFor(lastName, 'Last name changed.');
    }

    if(emailValue === ogEmailValue) {
        setWarningFor(email, 'No change made to email.');
    } else if (!isEmail(emailValue)) {
        setErrorFor(email, 'Invalid email address.');
        falseFlag = true;
    } else {
        setSuccessFor(email, 'Email changed.');
    }
    
    return !falseFlag;
}

function setErrorFor(input, message) {
    const formControl = input.parentElement;
    const small = formControl.querySelector('small');
    small.innerText = message;
    formControl.className = 'form-control error';
}

function setWarningFor(input, message) {
    const formControl = input.parentElement;
    const small = formControl.querySelector('small');
    small.innerText = message;
    formControl.className = 'form-control warning'
}

function setSuccessFor(input, message) {
    const formControl = input.parentElement;
    const small = formControl.querySelector('small');
    small.innerText = message;
    formControl.className = 'form-control success';
}

function isEmail(email) {
    return /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(email);
}

