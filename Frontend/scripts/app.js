//global variabelen
let SERVER_ENDPOINT = `http://www.omdbapi.com/?s=cars&apikey=b69563b7`;



//fetch
const fetchData = () => {
	SERVER_ENDPOINT = ``;
	// Add a few headers - UITZONDERING
	let customHeaders = new Headers();
	customHeaders.append('Accept', 'application/json');

	return fetch(SERVER_ENDPOINT, { headers: customHeaders })
		.then(r => r.json())
		.then(data => data);
};
//async function
const getApi = async function() {
	const data = await fetchData(SERVER_ENDPOINT);
	console.log(data);
};
//DOM
document.addEventListener('DOMContentLoaded', function() {
	console.info('DOM is ready to roll.. 👌');
	getApi();
});