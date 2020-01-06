//wacht tot de dom geladen is
// voor het togglen van het password gaan we ook een functie maken: handlePasswordSwitcher

const handlePasswordSwitcher = function(){
    //haal in deze functie de password-input en de password-toggle op

    const passwordInput= document.querySelector('.js-password-input'),
        passwordToggle= document.querySelector('.js-password-toggle');

    // we gaan luisteren of er geklikt wordt op de checkbox
    passwordToggle.addEventListener('change', function() {
        if(passwordInput.type === 'text'){
            passwordInput.type = 'password';
        } else{
            passwordInput.type= 'text';
        }

    });
    // als er geklikt wordt, veranderen we het type van de input van 'password' naar 'text' en vice versa. 
    
};

document.addEventListener('DOMContentLoaded', function(){
    handlePasswordSwitcher();
});