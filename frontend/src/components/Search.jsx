import React, { useState, useEffect, useContext } from "react";
import { Form, Radio, Input, Button, DatePicker, Select } from "antd";
import { fetchCity } from "../services/services";
import { GlobalContext } from "../context/GlobalStates";

const Search = () => {
  const [radio, setRadio] = useState(0);
  const [cities, setCities] = useState([]);
  const { setSearchText } = useContext(GlobalContext);
  useEffect(() => {
    fetchCity()
      .then((response) => {
        setCities(response);
      })
      .catch((error) => {
        console.log(error);
      });
  }, []);

  const radioStyle = {
    fontFamily: "sylfaen",
    height: "22px",
    lineHeight: "30px",
  };
  const datePicker = {
    width: "100%",
  };

  const onChange = (e) => {
    setSearchText("");
    search_form.resetFields();
    setRadio(e.target.value);
  };
  const onFinish = (values) => {
    console.log(values);
    if (radio === 0) {
      setSearchText(`searchText=${values.name}`);
    } else {
      values.dob = values.dob ? values.dob.format("YYYY-MM-DD") : undefined;
      setSearchText(
        Object.keys(values)
          .filter((key) => values[key])
          .map((key) => key + "=" + values[key])
          .join("&")
      );
    }
  };

  const onFinishFailed = (errorInfo) => {
    console.log("Failed:", errorInfo);
  };

  const [search_form] = Form.useForm();
  return (
    <div className="search-container">
      <Radio.Group onChange={onChange} value={radio}>
        <Radio style={radioStyle} value={0}>
          სწრაფი ძიება
        </Radio>
        <Radio style={radioStyle} value={1}>
          გაფართოებული ძიება
        </Radio>
      </Radio.Group>
      <div className="search-fields">
        <Form
          form={search_form}
          name="basic"
          initialValues={{ remember: true }}
          onFinish={onFinish}
          onFinishFailed={onFinishFailed}
        >
          {radio ? (
            <div>
              <Form.Item name="firstName">
                <Input placeholder="სახელი" />
              </Form.Item>
              <Form.Item name="lastName">
                <Input placeholder="გვარი" />
              </Form.Item>
              <Form.Item name="dob">
                <DatePicker style={datePicker} placeholder="დაბადების თარიღი" />
              </Form.Item>

              <Form.Item name="city">
                <Select placeholder="ქალაქი">
                  {cities.map((value, index) => (
                    <Select.Option key={index} value={index}>
                      {value.cityName}
                    </Select.Option>
                  ))}
                </Select>
              </Form.Item>

              <Form.Item name="phonenumber">
                <Input placeholder="ტელეფონის ნომერი" />
              </Form.Item>
            </div>
          ) : (
            <div>
              <Form.Item name="name">
                <Input placeholder="სახელი ან გვარი" />
              </Form.Item>
            </div>
          )}
          <div style={{ display: "flex", flexDirection: "row" }}>
            <Form.Item>
              <Button
                type="primary"
                htmlType="submit"
                style={{ marginRight: "10px" }}
              >
                ძიება
              </Button>
            </Form.Item>
            <Form.Item>
              <Button
                onClick={() => {
                  search_form.resetFields();
                  setSearchText("");
                }}
              >
                გასუფთავება
              </Button>
            </Form.Item>
          </div>
        </Form>
      </div>
    </div>
  );
};
export default Search;
