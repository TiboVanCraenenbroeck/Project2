//global variabelen
let SERVER_ENDPOINT = ``;


/* 
//fetch
const fetchData = () => {
	SERVER_ENDPOINT = `https://mctproject2.azurewebsites.net/api/v1/game/bef11ca2-3fb0-4bdf-90d2-2ad0be4787e6?cookie_id=mD8n3TZ6ib2ygIluJPpSBqfTMEap8pjds3kJfAGXGtzN6I2uiAAzO0ep0Xop3Erxi5wKjrntwvhOGe0hpTP6vfC/BL9F5Oxr9e8btDtzkpgkk3zg9yywlLpbmj82p/qDGIlYsMMOqsv7oz0hig06xw==`;
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
	//testing
	let test;
	test=encodeURIComponent("https://mctproject2.azurewebsites.net/api/v1/game/bef11ca2-3fb0-4bdf-90d2-2ad0be4787e6?cookie_id=mD8n3TZ6ib2ygIluJPpSBqfTMEap8pjds3kJfAGXGtzN6I2uiAAzO0ep0Xop3Erxi5wKjrntwvhOGe0hpTP6vfC/BL9F5Oxr9e8btDtzkpgkk3zg9yywlLpbmj82p/qDGIlYsMMOqsv7oz0hig06xw==")
	console.log(test);
	console.log(decodeURIComponent(test))
}; */


let data;
data={teams:[{name:"team C Test",avatar:{avatar_id:"69FBBDD6-BFBB-4802-8B0D-3E37350CED4C"}},{name:"team D Test",avatar:{avatar_id:"69FBBDD6-BFBB-4802-8B0D-3E37350CED4C"}}]}

const fetchData2 = async function( method = "POST", body = null) {
	return fetch(`https://mctproject2.azurewebsites.net/api/v1/game/BEF11CA2-3FB0-4BDF-90D2-2AD0BE4787E6`, {
	  method: method,
	  body: JSON.stringify(data),
	  headers: { "content-type": "application/json" }
	})
	  .then(r => r.json())
	  .then(data => data);
  };
  
  
const getAPI = async function(url, method = "POST", body = null) {
	try {
	  const data = await fetchData2(url, method, body);
	  console.log(data);
	} catch (error) {
	  console.log(error);
	}
  };
 


//DOM
document.addEventListener('DOMContentLoaded', function() {
	console.info('DOM is ready to roll.. 👌');
//getAPI();	

});



