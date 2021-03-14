import axios from "axios";

export const fetchContacts = (searchText) => {
  const url = `api/contacts${
    searchText.includes("searchText=") ? "" : "/searchByDetails"
  }?${searchText}`;
  return new Promise((resolve, reject) => {
    axios
      .get(url)
      .then((response) => {
        resolve(response.data);
      })
      .catch((error) => {
        reject(error);
      });
  });
};

export const fetchContact = (contactID) => {
  const url = `api/contacts/${contactID}`;
  return new Promise((resolve, reject) => {
    axios
      .get(url)
      .then((response) => {
        resolve(response.data);
      })
      .catch((error) => {
        reject(error);
      });
  });
};

export const fetchGender = () => {
  const url = `api/contacts/gender`;
  return new Promise((resolve, reject) => {
    axios
      .get(url)
      .then((response) => {
        resolve(response.data);
      })
      .catch((error) => {
        reject(error);
      });
  });
};

export const fetchPhoneNumberType = () => {
  const url = `api/contacts/phoneNumberTypes`;
  return new Promise((resolve, reject) => {
    axios
      .get(url)
      .then((response) => {
        resolve(response.data);
      })
      .catch((error) => {
        reject(error);
      });
  });
};

export const fetchCity = () => {
  const url = `api/contacts/city`;
  return new Promise((resolve, reject) => {
    axios
      .get(url)
      .then((response) => {
        resolve(response.data);
      })
      .catch((error) => {
        reject(error);
      });
  });
};

export const deleteContact = (contactID) => {
  const url = `api/contacts/${contactID}`;
  return new Promise((resolve, reject) => {
    axios
      .delete(url)
      .then((response) => {
        resolve(response);
      })
      .catch((error) => {
        reject(error);
      });
  });
};

export const postContact = (data, contactId) => {
  const url = `api/contacts` + (contactId ? "/" + contactId : "");
  const method = contactId ? "put" : "post";
  return new Promise((resolve, reject) => {
    axios[method](url, data)
      .then((response) => {
        resolve(response);
      })
      .catch((error) => {
        reject(error);
      });
  });
};

export const toggleFavorite = (contactID) => {
  const url = `api/contacts/addFavorite/${contactID}`;
  return new Promise((resolve, reject) => {
    axios
      .put(url)
      .then((response) => {
        resolve(response);
      })
      .catch((error) => {
        reject(error);
      });
  });
};

export const fetchDeleted = () => {
  const url = `api/contacts/deletedContacts`;
  return new Promise((resolve, reject) => {
    axios
      .get(url)
      .then((response) => {
        resolve(response.data);
      })
      .catch((error) => {
        reject(error);
      });
  });
};

export const fetchFavorites = () => {
  const url = `api/contacts/favoritedContacts`;
  return new Promise((resolve, reject) => {
    axios
      .get(url)
      .then((response) => {
        resolve(response.data);
      })
      .catch((error) => {
        reject(error);
      });
  });
};
