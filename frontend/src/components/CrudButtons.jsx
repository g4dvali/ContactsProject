import React, { useContext, useState } from "react";
import { Button, Modal } from "antd";
import { GlobalContext } from "../context/GlobalStates";
import AddForm from "./AddForm";
import DeleteContact from "./DeleteContact";
import { deleteContact } from "../services/services";

const CrudButtons = () => {
  const {
    isModalVisible,
    setIsModalVisible,
    isDeleteModalVisible,
    setIsDeleteModalVisible,
    contactID,
    setContactID,
    setIsEditing,
  } = useContext(GlobalContext);

  const [modalOpen, setModalOpen] = useState(false);

  const add = () => {
    setIsEditing(false);
    setIsModalVisible(true);
  };

  const edit = () => {
    if (contactID === 0) {
      setModalOpen(true);
    } else {
      setIsEditing(true);
      setIsModalVisible(true);
    }
  };
  const handleOk = () => {
    setIsModalVisible(false);
  };
  const handleCancel = () => {
    setIsModalVisible(false);
  };

  const showDeleteContactModal = () => {
    if (contactID === 0) {
      setModalOpen(true);
    } else {
      setIsDeleteModalVisible(true);
    }
  };
  const handleDeleteContactOk = () => {
    setIsDeleteModalVisible(false);
    deleteContact(contactID)
      .then((response) => {
        console.log(response);
        setContactID(0);
      })
      .catch((error) => {
        console.log(error);
      });
  };
  const handleDeleteContactCancel = () => {
    setIsDeleteModalVisible(false);
  };

  return (
    <div className="crud-buttons">
      <Modal
        title="ყურადღება"
        visible={modalOpen}
        onCancel={() => setModalOpen(false)}
        footer={[
          <Button
            key="submit"
            type="primary"
            onClick={() => setModalOpen(false)}
          >
            კარგი
          </Button>,
        ]}
      >
        <p>გთხოვთ, მონიშნოთ ჩანაწერი.</p>
      </Modal>

      <AddForm
        isVisible={isModalVisible}
        handleOk={handleOk}
        handleCancel={handleCancel}
      />

      <DeleteContact
        isVisible={isDeleteModalVisible}
        handleOk={handleDeleteContactOk}
        handleCancel={handleDeleteContactCancel}
      />

      <Button type="primary" onClick={add}>
        დამატება
      </Button>
      <Button onClick={edit}>რედაქტირება</Button>
      <Button onClick={showDeleteContactModal} danger>
        წაშლა
      </Button>
    </div>
  );
};

export default CrudButtons;
