let domBtnStartSpel, domBtnLeerkracht;
const btnStartSpel = () => {
	domBtnStartSpel.addEventListener('click', function() {
		window.location.href = 'avatar.html';
	});
};
const btnLeerkrachten = () => {
	domBtnLeerkracht.addEventListener('click', function() {
		window.location.href = 'leerkrachtenlogin.html';
	});
};
const init = () => {
	domBtnStartSpel = document.querySelector('.js-btnStart');
	domBtnLeerkracht = document.querySelector('.js-btnLeerkracht');
};
document.addEventListener('DOMContentLoaded', function() {
	console.log('Geladen😎');
	init();
	btnStartSpel();
	btnLeerkrachten();
});
