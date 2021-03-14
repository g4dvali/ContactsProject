import React, { useContext } from "react";
import { Button } from "antd";
import { GlobalContext } from "../context/GlobalStates";

const Filters = () => {
  const { setFilterType } = useContext(GlobalContext);

  return (
    <div className="crud-buttons">
      <Button onClick={() => setFilterType(1)}>ფავორიტების გამოტანა</Button>
      <Button onClick={() => setFilterType(2)}>წაშლილების გამოტანა</Button>
      <Button onClick={() => setFilterType(0)}>გასუფთავება</Button>
    </div>
  );
};

export default Filters;
