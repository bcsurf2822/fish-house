import axios from "axios";

const API_URL = "https://fishwebapp001-c6bvbebffqetg7gq.canadacentral-01.azurewebsites.net";

export const getAllFish = async () => {
  try {
    const response = await axios.get(`${API_URL}/api/fish/getall`);
    console.log(response.data);
    return response.data;
  } catch (error) {
    console.error("Error fetching market data:", error);
    throw error;
  }
};

export const getFishForInventory = async () => {
  try {
    const response = await axios.get(`${API_URL}/api/fish/fishinventory`);
    console.log(response.data);
    return response.data;
  } catch (error) {
    console.error("Error fetching market data:", error);
    throw error;
  }
};

export const updateFishPrice = async (id, newPrice) => {
  const token = localStorage.getItem("authToken"); 

  if (!token) {
    throw new Error("No token found. Please log in first.");
  }``

  try {
    const response = await axios.patch(
      `${API_URL}/api/fish/updatepartial/${id}`,
      [
        {
          operationType: 0,
          path: "/price",
          op: "replace",
          value: newPrice,
        },
      ],
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );

    console.log("Fish price updated successfully!");
    return response.data;
  } catch (error) {
    const errorMessage =
      error.response?.data || "Failed to update fish price. Please try again.";
    console.error("Error updating fish price:", errorMessage);
    throw error;
  }
};
