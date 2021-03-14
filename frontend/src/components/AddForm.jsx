import React, { useState, useContext, useEffect } from "react";
import { Form, Modal, Checkbox, Input, DatePicker, Select, Button } from "antd";
import { GlobalContext } from "../context/GlobalStates";
import {
  fetchContact,
  fetchCity,
  fetchGender,
  postContact,
  fetchPhoneNumberType,
} from "../services/services";
import moment from "moment";
const { TextArea } = Input;
const AddForm = ({ isVisible, handleOk, handleCancel }) => {
  const [cities, setCities] = useState([]);
  const [gender, setGender] = useState([]);
  const [phoneNumberType, setPhoneNumberType] = useState([]);
  const [contact, setContact] = useState({});
  const [otherCity, setOtherCity] = useState(false);
  const {
    contactID,
    isModalVisible,
    isEditing,
    setIsModalVisible,
    setContactID,
  } = useContext(GlobalContext);
  const [date, setDate] = useState(null);
  useEffect(() => {
    fetchPhoneNumberType()
      .then((response) => {
        setPhoneNumberType(response);
      })
      .catch((error) => {
        console.log(error);
      });

    fetchCity()
      .then((response) => {
        setCities(response);
      })
      .catch((error) => {
        console.log(error);
      });

    fetchGender()
      .then((response) => {
        setGender(response);
      })
      .catch((error) => {
        console.log(error);
      });

    if (contactID > 0) {
      fetchContact(contactID)
        .then((response) => {
          setContact(response);
        })
        .catch((error) => {
          console.log(error);
        });
    } else {
      setContact({});
    }
    console.log(contact);
  }, [contactID]);

  const onFinish = (values) => {
    const data = {
      ...values,
      dob: date,
      phoneNumbersPost: values.phoneNumbers,
    };
    data.phoneNumbers = data.phoneNumbersPost = Object.values(
      data.phoneNumbers
    ).filter((item) => {
      return item.phoneNumber && item.phoneNumberTypeID;
    });
    delete data.PhoneNumberTypeID;
    delete data.PhoneNumber;
    postContact(data, isEditing ? contactID : undefined)
      .then((response) => {
        setIsModalVisible(false);
        setContactID(0);
      })
      .catch((error) => {
        if (typeof error.response.data === "string") {
          alert(error.response.data);
        } else if (
          typeof error.response.data === "object" &&
          error.response.data?.errors
        ) {
          const msg = Object.keys(error.response.data.errors)
            .map(
              (key) => key + ": " + error.response.data.errors[key].join("; ")
            )
            .join("\n");
          alert(msg);
        } else {
          alert("Something went wrong!!");
        }
      });
  };

  const onFinishFailed = (errorInfo) => {
    console.log("Failed:", errorInfo);
  };
  const datePicker = {
    width: "100%",
  };

  const handleDate = (date, dateString) => {
    setDate(dateString);
    console.log(date, dateString);
  };

  useEffect(() => {
    console.log(isModalVisible);
  }, [isModalVisible]);

  useEffect(() => {
    setDate(contact.dob.format("YYYY-MM-DD"));
  }, [contact]);

  contact.dob = contact ? moment(contact.dob) : null;

  contact.phoneNumbers = contact.phoneNumbers
    ? contact.phoneNumbers
    : [
        {
          phoneNumberTypeID: null,
          phoneNumber: null,
        },
      ];
  return (
    isModalVisible && (
      <Modal
        title="კონტაქტის დამატება/რედაქტირება"
        visible={isVisible}
        onOk={handleOk}
        onCancel={handleCancel}
        footer={false}
      >
        <Form
          name="basic"
          initialValues={{
            additionalDetails: null,
            address: null,
            cityActionID: false,
            cityID: null,
            cityName: null,
            dob: null,
            firstname: null,
            genderID: null,
            isFavorite: false,
            lastname: null,
            otherPhoneNumber: null,
            PhoneNumber: null,
            phoneNumberTypeActionID: false,
            phoneNumberTypeID: null,
            phoneNumberTypeName: null,
            phoneNumber: null,
            phoneNumbers: [],
            phoneNumbersPost: [],
            ...(isEditing ? contact : {}),
          }}
          onFinish={onFinish}
          onFinishFailed={onFinishFailed}
        >
          <div>
            <Form.Item name="firstname">
              <Input placeholder="სახელი" />
            </Form.Item>

            <Form.Item name="lastname">
              <Input placeholder="გვარი" />
            </Form.Item>

            <Form.Item name="genderID">
              <Select placeholder="სქესი" allowClear>
                {gender.map((value, index) => (
                  <Select.Option key={index} value={value.id}>
                    {value.genderName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>

            <Form.Item name="dob">
              <DatePicker
                style={datePicker}
                placeholder="დაბადების თარიღი"
                onChange={handleDate}
              />
            </Form.Item>

            <Form.Item name="cityID">
              <Select placeholder="ქალაქი" allowClear>
                {cities.map((value, index) => (
                  <Select.Option key={index} value={value.id}>
                    {value.cityName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <div style={{ display: "flex", flexDirection: "row" }}>
              <Form.Item
                name="cityActionID"
                style={{ width: "100%" }}
                valuePropName="checked"
              >
                <Checkbox onChange={() => setOtherCity(!otherCity)}>
                  დაამატე ქალაქი
                </Checkbox>
              </Form.Item>
              <Form.Item name="cityName" style={{ width: "100%" }}>
                <Input placeholder="დაამატეთ ქალაქი" disabled={!otherCity} />
              </Form.Item>
            </div>
            <Form.Item name="address">
              <Input placeholder="მისამართი" />
            </Form.Item>
            <Form.Item name="additionalDetails">
              <TextArea row={4} placeholder="დამატებითი ინფორმაცია" />
            </Form.Item>

            <button
              onClick={(e) => {
                contact.phoneNumbers.push({
                  phoneNumberTypeID: null,
                  phoneNumber: null,
                  id: 0,
                });
                setContact({ ...contact });
                e.preventDefault();
              }}
              style={{
                "margin-bottom": "10px",
              }}
              class="ant-btn ant-btn-primary"
            >
              +
            </button>

            {contact.phoneNumbers.map((item, index) => (
              <>
                <div style={{ display: "flex", flexDirection: "row" }}>
                  <Form.Item
                    name={`phoneNumbers.${index}.phoneNumberTypeID`.split(".")}
                    style={{ width: "100%", marginRight: "10px" }}
                  >
                    <Select placeholder="ტიპი" allowClear>
                      {phoneNumberType.map((value, index) => (
                        <Select.Option key={index} value={value.id}>
                          {value.typeName}
                        </Select.Option>
                      ))}
                    </Select>
                  </Form.Item>
                  <Form.Item
                    name={`phoneNumbers.${index}.phoneNumber`.split(".")}
                    style={{ width: "100%" }}
                  >
                    <Input placeholder="შეიყვანეთ ნომერი" />
                  </Form.Item>
                  <Form.Item
                    name={`phoneNumbers.${index}.id`.split(".")}
                    hidden
                  >
                    <Input placeholder="ID" />
                  </Form.Item>

                  <button
                    onClick={(e) => {
                      delete contact.phoneNumbers[index];
                      setContact({ ...contact });
                      e.preventDefault();
                    }}
                    class="ant-btn ant-btn-dangerous"
                    style={{
                      height: "30px",
                      padding: "0px 10px",
                    }}
                  >
                    -
                  </button>
                </div>
              </>
            ))}

            <Form.Item name="phoneNumberTypeActionID" valuePropName="checked">
              <Checkbox>დაამატე სხვა ტელეფონის ნომერი</Checkbox>
            </Form.Item>
            <div style={{ display: "flex", flexDirection: "row" }}>
              <Form.Item
                name="phoneNumberTypeName"
                style={{ width: "100%", marginRight: "10px" }}
              >
                <Input placeholder="დაამატეთ ტიპი" />
              </Form.Item>
              <Form.Item name="otherPhoneNumber" style={{ width: "100%" }}>
                <Input placeholder="დაამატეთ ნომერი" />
              </Form.Item>
            </div>
            <Form.Item name="isFavorite" valuePropName="checked">
              <Checkbox>ფავორიტი</Checkbox>
            </Form.Item>
            <Form.Item>
              <Button type="primary" htmlType="submit">
                შენახვა
              </Button>
            </Form.Item>
          </div>
        </Form>
      </Modal>
    )
  );
};

export default AddForm;
