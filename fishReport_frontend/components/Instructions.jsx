import React from 'react';

const Instructions = () => {
  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-cyan-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-4xl mx-auto">
        {/* Header */}
        <div className="text-center mb-12">
          <h1 className="text-4xl font-bold text-gray-900 mb-4">
            Federal Fish Market Control System
          </h1>
          <p className="text-xl text-gray-600 italic">
            Demo Application
          </p>
        </div>

        {/* Main Content Card */}
        <div className="bg-white rounded-lg shadow-xl p-8 mb-8">
          {/* Introduction */}
          <div className="mb-8">
            <h2 className="text-2xl font-semibold text-gray-800 mb-4">
              Welcome, Market Administrator
            </h2>
            <p className="text-gray-700 leading-relaxed mb-4">
 As a <span className="font-semibold">Fish Market Administrator</span>,
              you now wield the power to create markets, set prices, and control the entire fish supply chain.
            </p>
            <p className="text-gray-700 leading-relaxed">
              This is a demo application for a fictional fish market management system.
            </p>
          </div>

          {/* Authentication Section */}
          <div className="bg-yellow-50 border-l-4 border-yellow-400 p-6 mb-8">
            <h3 className="text-lg font-semibold text-yellow-900 mb-3">
              Authentication Required
            </h3>
            <p className="text-yellow-800 mb-3">
              To perform any administrative actions, you must be logged in with the following credentials:
            </p>
            <div className="bg-white rounded p-4 font-mono text-sm">
              <div className="mb-2">
                <span className="text-gray-600">Username:</span>{' '}
                <span className="font-semibold text-gray-900">marketAdmin</span>
              </div>
              <div>
                <span className="text-gray-600">Password:</span>{' '}
                <span className="font-semibold text-gray-900">marketAdmin</span>
              </div>
            </div>
          </div>

          {/* Instructions Sections */}
          <div className="space-y-8">
            {/* Markets Tab */}
            <div>
              <h3 className="text-xl font-semibold text-gray-800 mb-4 flex items-center">
                <span className="bg-blue-500 text-white rounded-full w-8 h-8 flex items-center justify-center mr-3">
                  1
                </span>
                Markets Tab
              </h3>
              <div className="ml-11 space-y-3">
                <div className="bg-gray-50 rounded-lg p-4">
                  <h4 className="font-semibold text-gray-800 mb-2">Add a Market</h4>
                  <ul className="list-disc list-inside text-gray-700 space-y-1">
                    <li>Click the "Add Market" button (login required)</li>
                    <li>Enter the market name and location</li>
                    <li>Submit to create your new government-controlled market</li>
                  </ul>
                </div>

                <div className="bg-gray-50 rounded-lg p-4">
                  <h4 className="font-semibold text-gray-800 mb-2">Add Fish to a Market</h4>
                  <ul className="list-disc list-inside text-gray-700 space-y-1">
                    <li>Select a market from the list on the left</li>
                    <li>Click "Add Fish" at the bottom of the market details</li>
                    <li>Select a fish species from the dropdown menu</li>
                    <li>Click "Add Fish" to add it to the market's inventory</li>
                  </ul>
                </div>

                <div className="bg-gray-50 rounded-lg p-4">
                  <h4 className="font-semibold text-gray-800 mb-2">Remove Fish from a Market</h4>
                  <ul className="list-disc list-inside text-gray-700 space-y-1">
                    <li>Select a market to view its current inventory</li>
                    <li>Click the "Delete" button next to any fish species</li>
                    <li>Confirm the removal to update the inventory</li>
                  </ul>
                </div>

                <div className="bg-gray-50 rounded-lg p-4">
                  <h4 className="font-semibold text-gray-800 mb-2">Delete a Market</h4>
                  <ul className="list-disc list-inside text-gray-700 space-y-1">
                    <li>Select the market you want to eliminate</li>
                    <li>Click "Delete Market" at the top of the market details</li>
                    <li>Confirm deletion to permanently remove the market</li>
                  </ul>
                </div>
              </div>
            </div>

            {/* Fish Tab */}
            <div>
              <h3 className="text-xl font-semibold text-gray-800 mb-4 flex items-center">
                <span className="bg-blue-500 text-white rounded-full w-8 h-8 flex items-center justify-center mr-3">
                  2
                </span>
                Fish Tab
              </h3>
              <div className="ml-11 space-y-3">
                <div className="bg-gray-50 rounded-lg p-4">
                  <h4 className="font-semibold text-gray-800 mb-2">View All Fish Species</h4>
                  <p className="text-gray-700 mb-2">
                    Browse through all 18 available fish species with detailed information including:
                  </p>
                  <ul className="list-disc list-inside text-gray-700 space-y-1">
                    <li>Habitat</li>
                    <li>Length (cm)</li>
                    <li>Population estimate</li>
                    <li>Average lifespan (years)</li>
                    <li>Current market price (per lb)</li>
                  </ul>
                </div>

                <div className="bg-gray-50 rounded-lg p-4">
                  <h4 className="font-semibold text-gray-800 mb-2">Edit Fish Prices</h4>
                  <ul className="list-disc list-inside text-gray-700 space-y-1">
                    <li>Login required to edit prices</li>
                    <li>Click "Edit Price" on any fish card</li>
                    <li>Enter your new government-mandated price</li>
                    <li>Click "Save" to update the price across all markets</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>


        </div>



        {/* Footer Note */}
        <div className="text-center mt-8 text-gray-500 text-sm">
          <p>This is a demonstration application.</p>
        </div>
      </div>
    </div>
  );
};

export default Instructions;
