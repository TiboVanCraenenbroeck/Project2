let teamAName,teamBName,couter=0;
let dataName;

const teamA =()=>{
    let volgende= document.querySelector(".js-volgende")
    let test2= document.querySelector(".js-test")
    volgende.addEventListener('click', function() {
    let teamNaam=document.getElementById('username').value

        console.log("klik");
        //console.log(teamNaam)

        if(couter==1)
        {
            teamBName=teamNaam;
            /* let teamBHTML='';
            teamBHTML=`<p class="js-test">${teamAName},${teamBName}</p>`;
            test2.innerHTML = teamBHTML;  */
            console.log(teamAName,teamBName); 
            dataName={teams:[{name:teamAName,avatar:{avatar_id:"69FBBDD6-BFBB-4802-8B0D-3E37350CED4C"}},{name:teamBName,avatar:{avatar_id:"69FBBDD6-BFBB-4802-8B0D-3E37350CED4C"}}]}
            getAPI(dataName);
            teamAName="";
            teamBName="";
            couter=0;
         
        }
        else
        {
            
            let teamBHTML='';
            teamBHTML=`<a class="js-volgende" href="#">Start Spel</a>`;
            volgende.innerHTML = teamBHTML;  
            teamAName=teamNaam;
            couter=1;
          
        }
    });
}



const fetchData2 = async function( dataName,method = "POST", body = null) {
    console.log("in fetch")
	return fetch(`https://mctproject2.azurewebsites.net/api/v1/game/BEF11CA2-3FB0-4BDF-90D2-2AD0BE4787E6`, {
	  method: method,
	  body: JSON.stringify(dataName),
	  headers: { "content-type": "application/json" }
	})
	  .then(r => r.json())
	  .then(data => data);
  };
  
  
const getAPI = async function(dataName,url, method = "POST", body = null) {
	try {
      const dataURL = await fetchData2(dataName,url, method, body);
      console.log("in try")
	  console.log(dataURL);
	} catch (error) {
	  console.log(error);
	}
  };
 

//DOM
document.addEventListener('DOMContentLoaded', function() 
{
	console.info('DOM is ready to roll.. 👌');
    teamA();

});