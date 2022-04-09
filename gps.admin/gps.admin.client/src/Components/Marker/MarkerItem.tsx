import { useMapEvent } from "react-leaflet";

export function MarkerItem({onMapClick}: any) {
    
    const map = useMapEvent("click", (e)=>{
        
        onMapClick(e);
        return null;
    });
  
    return null;
  }