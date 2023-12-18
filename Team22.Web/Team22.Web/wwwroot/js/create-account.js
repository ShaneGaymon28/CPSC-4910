const form = document.getElementById('form');
const firstName = document.getElementById('first-name');
const lastName = document.getElementById('last-name');
const email = document.getElementById('email');
const password = document.getElementById('password');
const confirmPassword = document.getElementById('confirm-password');
const userRole = document.getElementById('user-role');
const orgField = document.getElementById('sponsor-org-field');
const sponsorOrg = document.getElementById('sponsor-org');
let sponsorFlag = false;

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
    const passwordValue = password.value.trim();
    const confirmPasswordValue = confirmPassword.value.trim();
    
    let falseFlag = false;

    if(sponsorFlag) {
        const sponsorValue = sponsorOrg.value;

        if(sponsorValue == ''){
            setErrorFor(sponsorOrg,);
            falseFlag = true;
        } else {
            setSuccessFor(sponsorOrg);
        }
    }

    if(firstNameValue == '') {
        setErrorFor(firstName, );
        falseFlag = true;
    }
    else {
        setSuccessFor(firstName);
    }

    if(lastNameValue == '') {
        setErrorFor(lastName, );
        falseFlag = true;
    }
    else {
        setSuccessFor(lastName);
    }

    if(emailValue == '') {
        setErrorFor(email, );
        falseFlag = true;
    } else if (!isEmail(emailValue)) {
        setErrorFor(email, );
        falseFlag = true;
    } else {
        setSuccessFor(email);
    }

    if(passwordValue == '') {
        setErrorFor(password, );
        falseFlag = true;
    } else {
        setSuccessFor(password);
    }

    if(confirmPasswordValue == ''){
        setErrorFor(confirmPassword, );
        falseFlag = true;
    } else if (passwordValue != confirmPasswordValue) {
        setErrorFor(confirmPassword, );
        falseFlag = true;
    } else {
        setSuccessFor(confirmPassword);
    }
    
    return !falseFlag;
}

function setErrorFor(input) {
    const formControl = input.parentElement;
    const small = formControl.querySelector('span');
    formControl.className = 'form-control error';
}

function setSuccessFor(input) {
    const formControl = input.parentElement;
    formControl.className = 'form-control success';
}

function isEmail(email) {
    return /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(email);
}

