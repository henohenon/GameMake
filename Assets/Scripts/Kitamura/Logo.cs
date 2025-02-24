using System.Collections;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class Logo : MonoBehaviour
{
    private UIDocument uiDocument;
    private static string logoText = "";

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        
        var label = uiDocument.rootVisualElement.Q<Label>("label");
        LMotion.String.Create512Bytes("", logoText, 2f).
            WithScrambleChars(ScrambleMode.Numerals)
            .BindToText(label);
    }
    
}

/*
          `  `    `         `     ` ..Z""7"TG.. `  `     `           `  `
       `                 `        .d=         ?h.          `  `  `
  `  `         `    `  `    `  ` J=             4,   `  `           `
          `  `   `           (MNNMNMa,  `  `     ?h,     (-..   `       `
                           ..MMMMMMMML.            ?"TOgc<(<.      `
  `  `  `      `    `  ..HHHHHH@@H@@@HHHm..   `        _~++<   `     `  `
           `     `  .(MHHH@H@@@MHgHHHHgH@@HN,            `        `
                  .(HH@HHHHHHHHMHHHMHHkHHHH@@N,
     `       (lli.MHH@H@H@H@M8tttMHHHHHNU6lllllti.    `       `
          `  (llllWHHHHH@HH#IllllMHH@H#lllluueszlll-             `  `   `
             (lllllvMH@HHM8llllllMHHH#llldHNbqg@klll-    `               
  `    `     (lldmlllW@HM0lluKlllMH@H#lllWHMHq@@Hm2?!        `           
             (lldHNylld8llldH@lllMHH@MslllOV9WHMHHN            `   `     
             (lldHHMmlllluHHH@lllMHHHHHmsllllllllZW.    `             `  
     `    `  (lldHHHHNyldHH@H@lllMH@H@HHHHMHmgeyllll.      `             
             (lldHHH##MHHHH@H@lllMHHMMMMMHHHHH@HNyllz          `  `      
             (lldHH####HHH@HH@lllMHHMOlldMHH@@HHM6llv `  `           `   
        `    (lldHH#NN##HHH@H@lllMHHHNzlllVWHHB9Ollv!                    
    `        (llvMHH#NN##MHHH@lllMHH@HMmyllllllllv!         `  `  `      
                 ,MH#NNNN#######HHHHH@HH@HMHHHHF         `               
                   7MH##N#NNN#NNNN#HHH@@HHHHH#'                    `     
       `    `        ?HHHH#####H##HHHH@HH@MY^         `       `          
    `          `        7WMHHHHHHHH@H@H#"^        `      `       `       
                             ?7"""7=!                               `    
 */
