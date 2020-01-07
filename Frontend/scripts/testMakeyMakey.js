//DOM

const makeymakey = () => {
	document.addEventListener('keyup', function(key) {
		//keyup omdat je geen probleem zou hebben met het lan inihouden van een bepaalde toets
		switch (key.keyCode) {
			case 37:
				console.log('left');
				counter = 1;
				break;
			case 38:
				console.log('up');
				counter = 1;
				break;
			case 39:
				console.log('right');
				counter = 1;
				break;
			case 40:
				console.log('down');
				counter = 1;
				break;
		}
	});
};

document.addEventListener('DOMContentLoaded', function() {
	console.info('DOM is ready to roll.. 👌');
	makeymakey();
});
