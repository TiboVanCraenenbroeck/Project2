let teamAName,teamBName,couter=0,teamAAvatarId="69fbbdd6-bfbb-4802-8b0d-3e37350ced3d",teamBAvatarId="69fbbdd6-bfbb-4802-8b0d-3e37350ced3c";
let dataName;
let teamNamechange; 




const raketKeuze=()=>{
    let raket= document.querySelector(".js-raket");
    raket.addEventListener('click', function() {
        console.log(raket.value)

    });

}
const teamA =()=>{
    let volgende= document.querySelector(".js-volgende")
    let naamverandering = document.querySelector(".js-name");
    volgende.addEventListener('click', function() {
    let teamNaam=document.getElementById('username').value
        console.log("klik");
        //console.log(teamNaam)

        if(couter==1)
        {
            teamBName=teamNaam;
            console.log(teamAName,teamBName);
            if (typeof(Storage) !== "undefined") {
                // Store
                localStorage.setItem('teamnaamA',teamAName); 
                localStorage.setItem('teamnaamB',teamBName); 
                localStorage.setItem('teamAvatarA',teamAAvatarId); 
                localStorage.setItem('teamAvatarB',teamBAvatarId); 


                let teamnaamA,teamnaamB,teamAvatarA,teamAvatarB;
                teamnaamA=localStorage.getItem('teamnaamA');
                teamnaamB=localStorage.getItem('teamnaamB');
                teamAvatarA=localStorage.getItem('teamAvatarA');
                teamAvatarB=localStorage.getItem('teamAvatarB');
                //console.log("teama: "+localStorage.getItem('teamnaamA')+" teamb: "+localStorage.getItem('teamnaamB')+" avatara: "+localStorage.getItem('teamAvatarA')+" avatarb: "+localStorage.getItem('teamAvatarB'))
                // Retrieve
              }
            teamAName="";
            teamBName="";
            couter=0;
         
        }
        else
        {
            let teamBHTML='';
            let teamNamechange = '';
            teamNamechange = `<h4>Team naam groep 2</h4>`
            teamBHTML=`<a class="js-volgende" href="./loadscreen.html">Start Spel</a>`;
            volgende.innerHTML = teamBHTML;
            naamverandering.innerHTML = teamNamechange;
            teamAName=teamNaam;
            couter=1;
          
        }
    });
}




 

//DOM
document.addEventListener('DOMContentLoaded', function() 
{
	console.info('DOM is ready to roll.. 👌');
    teamA();
    raketKeuze();

});