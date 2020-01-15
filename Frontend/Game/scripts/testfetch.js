//fetch
let SERVER_ENDPOINT = ``;
const baseURL = "https://mctproject2.azurewebsites.net/api/v1";
let mail = "tibo.van.craenenbroeck@student.howest.be";
let password = "Test.1234";

const fetchData = (test) => {
	SERVER_ENDPOINT = ``;
	// Add a few headers - UITZONDERING
	let customHeaders = new Headers();
	customHeaders.append('Accept', 'application/json');

	return fetch(test, { headers: customHeaders })
		.then(r => r.json())
		.then(data => data);
};
//async function
const getApi = async function(SERVER_ENDPOINT) {
	const data = await fetchData(SERVER_ENDPOINT);
    console.log(data);
	//testing
}; 

document.addEventListener('DOMContentLoaded', function() {
	console.info('DOM is ready to roll.. 👌');
    getApi(`${baseURL}/login/${mail}/${password}`);	

});