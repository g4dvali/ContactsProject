import React, { useState, useEffect, useContext } from "react";
import { Table } from "antd";
import { fetchContacts } from "../services/services";
import {
  toggleFavorite,
  fetchDeleted,
  fetchFavorites,
} from "../services/services";
import { GlobalContext } from "../context/GlobalStates";
import moment from "moment";

const ContactList = () => {
  const [data, setData] = useState([]);
  const {
    searchText,
    setContactID,
    contactID,
    isModalVisible,
    filterType,
  } = useContext(GlobalContext);
  const columns = [
    {
      title: "სახელი და გვარი",
      dataIndex: "fullname",
    },
    {
      title: "მობილურის ნომერი",
      dataIndex: "phoneNumber",
    },
    {
      title: "ქალაქი",
      dataIndex: "cityName",
    },
    {
      title: "სქესი",
      dataIndex: "genderName",
    },
    {
      title: "მისამართი",
      dataIndex: "address",
    },
    {
      title: "დაბადების თარიღი",
      dataIndex: "dob",
      render(dob, record) {
        return dob ? moment(dob).format("YYYY-MM-DD") : "-";
      },
    },
    {
      title: "დამატებითი დეტალები",
      dataIndex: "additionalDetails",
    },
    {
      title: "ფავორიტი",
      dataIndex: "isFavorite",
      render(isFavorite, record) {
        return {
          children: (
            <button
              onClick={() => {
                toggleFavorite(record.contactID).then((response) => {
                  fetchAndSetContacts(searchText);
                });
              }}
            >
              {isFavorite ? "Remove" : "Add"}
            </button>
          ),
        };
      },
    },
  ];

  const fetchAndSetContacts = (searchText) => {
    fetchContacts(searchText)
      .then((response) => {
        const res = response.map((value) => ({
          ...value,
          key: value.contactID,
        }));
        setData(res);
      })
      .catch((error) => {
        console.log(error);
      });
  };

  useEffect(() => {
    if (filterType === 0) {
      fetchAndSetContacts(searchText);
    } else if (filterType === 1) {
      fetchFavorites()
        .then((response) => {
          const res = response.map((value) => ({
            ...value,
            key: value.contactID,
          }));
          setData(res);
        })
        .catch((error) => {
          console.log(error);
        });
    } else if (filterType === 2) {
      fetchDeleted()
        .then((response) => {
          const res = response.map((value) => ({
            ...value,
            key: value.contactID,
          }));
          setData(res);
        })
        .catch((error) => {
          console.log(error);
        });
    }
  }, [searchText, contactID, filterType]);

  useEffect(() => {
    if (!isModalVisible) {
      fetchAndSetContacts(searchText);
    }
  }, [isModalVisible, searchText]);

  return (
    <Table
      columns={columns}
      dataSource={data}
      pagination={false}
      size="middle"
      rowSelection={{
        type: "radio",
        onChange: (selectedRowKeys) => {
          console.log(selectedRowKeys);
          setContactID(selectedRowKeys);
        },
      }}
    />
  );
};

export default ContactList;
