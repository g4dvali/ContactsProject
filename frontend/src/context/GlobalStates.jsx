import React, { createContext, useState } from "react";

export const GlobalContext = createContext();

export const GlobalProvider = (props) => {
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [isDeleteModalVisible, setIsDeleteModalVisible] = useState(false);
  const [searchText, setSearchText] = useState("");
  const [contactID, setContactID] = useState(0);
  const [filterType, setFilterType] = useState(0);
  return (
    <GlobalContext.Provider
      value={{
        isModalVisible,
        setIsModalVisible,
        isDeleteModalVisible,
        setIsDeleteModalVisible,
        searchText,
        setSearchText,
        contactID,
        setContactID,
        isEditing,
        setIsEditing,
        filterType,
        setFilterType,
      }}
    >
      {props.children}
    </GlobalContext.Provider>
  );
};
