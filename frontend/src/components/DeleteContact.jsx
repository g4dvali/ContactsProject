import React from 'react'
import { Modal } from 'antd'

const DeleteContact = ({ isVisible, handleOk, handleCancel }) => {
    return (
        <Modal
            title="კონტაქტის წაშლა"
            visible={isVisible}
            onOk={handleOk}
            onCancel={handleCancel}
        >
            <p>წაიშალოს მონიშნული კონტაქტი?</p>
        </Modal>
    )
}

export default DeleteContact
