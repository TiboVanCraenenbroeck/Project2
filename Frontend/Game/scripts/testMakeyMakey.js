//DOM

const makeymakey = () => {
	document.addEventListener('keyup', function(key) {
		//keyup omdat je geen probleem zou hebben met het lan inihouden van een bepaalde toets
		switch (key.keyCode) {
			case 37:
				console.log('left');
				break;
			case 38:
				console.log('up');
				break;
			case 39:
				console.log('right');
				break;
			case 40:
				console.log('down');
				break;
			case 13:
				console.log('enter');
				break;
		}
	});
};

document.addEventListener('DOMContentLoaded', function() {
	console.info('DOM is ready to roll.. 👌');
	makeymakey();
});
